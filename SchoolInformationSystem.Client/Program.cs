using SchoolInformationSystem.Client;
using SchoolInformationSystem.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Kimlik doðrulama servislerini DI container'a ekle.
builder.Services.AddAuthorizationCore();

// HttpClient'a artýk ihtiyaç duymadýðý için bu kaydý basitleþtiriyoruz.
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthHttpHandler>();

// 2. API ile iletiþim kuracak HttpClient'ý yapýlandýr.
// Bu kayýt doðru ve döngü oluþturmaz.
builder.Services.AddHttpClient("API", client =>
  //  client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    client.BaseAddress = new Uri("https://localhost:7289/"))
    .AddHttpMessageHandler<AuthHttpHandler>();

// 3. Bileþenlerin HttpClient'ý kolayca enjekte edebilmesi için bu kaydý ekliyoruz.
// Bu satýr hatanýn kaynaðýydý ve þimdi döngü kýrýldýðý için doðru çalýþacak.
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

await builder.Build().RunAsync();