using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace SchoolInformationSystem.Client.Auth
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly HttpClient _refreshTokenClient;

        public AuthHttpHandler(CustomAuthStateProvider authStateProvider, NavigationManager navigationManager)
        {
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
            _refreshTokenClient = new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authStateProvider.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                try
                {
                    var refreshResponse = await _refreshTokenClient.PostAsync("api/auth/refresh", null, cancellationToken);
                    if (refreshResponse.IsSuccessStatusCode)
                    {
                        var newTokens = await refreshResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
                        await _authStateProvider.MarkUserAsAuthenticated(newTokens.AccessToken);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);
                        return await base.SendAsync(request, cancellationToken);
                    }
                }
                catch { /* Hata loglanabilir */ }

                // Herhangi bir refresh hatasında logout yap ve login'e yönlendir.
                await _authStateProvider.MarkUserAsLoggedOut();
                _navigationManager.NavigateTo("/login");
            }
            return response;
        }

        private record AccessTokenResponse(string AccessToken);
    }
}