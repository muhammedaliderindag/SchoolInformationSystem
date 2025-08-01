using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    // HttpClient'ı buradan kaldırıyoruz.
    // private readonly HttpClient _httpClient;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    // Constructor'dan HttpClient parametresini kaldırıyoruz.
    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(_anonymous);
            }

            // Artık HttpClient'ın başlığını burada ayarlamıyoruz. Bu işi AuthHttpHandler yapacak.
            // _httpClient.DefaultRequestHeaders.Authorization = ...

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwtAuth"));
            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    // MarkUserAsAuthenticated metodunda HttpClient ile ilgili bir işlem yoktu, aynen kalabilir.
    public async Task MarkUserAsAuthenticated(string token)
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "accessToken", token);
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwtAuth"));
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    // MarkUserAsLoggedOut metodunda HttpClient başlığını temizliyorduk, bu satırı kaldırıyoruz.
    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "accessToken");
        // _httpClient.DefaultRequestHeaders.Authorization = null; // Bu satırı kaldırın.
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    // Diğer yardımcı metotlar (GetTokenAsync, ParseClaimsFromJwt vb.) aynen kalabilir.
    public async Task<string> GetTokenAsync()
        => await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "accessToken");

    //private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    //    {
    //        var claims = new List<Claim>();
    //        var payload = jwt.Split('.')[1];
    //        var jsonBytes = ParseBase64WithoutPadding(payload);
    //        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
    //        if (keyValuePairs == null) return claims;

    //        keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out object? sub);
    //        if (sub != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString()));

    //        keyValuePairs.TryGetValue(ClaimTypes.Email, out object? email);
    //        if (email != null) claims.Add(new Claim(ClaimTypes.Email, email.ToString()));


    //        keyValuePairs.TryGetValue(ClaimTypes.Role, out object? role);
    //        if (role != null) claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

    //        return claims;
    //    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);

        // JSON'dan gelen tüm anahtar-değer çiftlerini alıyoruz.
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null)
        {
            return claims; // Eğer payload boşsa, boş liste dön.
        }

        // "sub" anahtarını bulup, .NET standardı olan ClaimTypes.NameIdentifier olarak ekliyoruz.
        // Bu, uygulamanın geri kalanında user.FindFirst(ClaimTypes.NameIdentifier) ile erişimi sağlar.
        if (keyValuePairs.TryGetValue("sub", out object id))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
        }

        // "email" anahtarını bulup, ClaimTypes.Email olarak ekliyoruz.
        if (keyValuePairs.TryGetValue("email", out object email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
        }

        // "role" anahtarını bulup, ClaimTypes.Role olarak ekliyoruz.
        // Bu kod hem tek bir rolü (string) hem de birden çok rolü (array) destekler.
        if (keyValuePairs.TryGetValue("role", out object roles))
        {
            if (roles is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in jsonElement.EnumerateArray())
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.GetString()));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }
        }

        // Diğer tüm claim'leri (exp, iat, custom claims vb.) de listeye ekleyebiliriz.
        // Bu genellikle iyi bir pratiktir.
        var otherClaims = keyValuePairs.Where(kvp => kvp.Key != "sub" && kvp.Key != "email" && kvp.Key != "role");
        claims.AddRange(otherClaims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
