using SchoolInformationSystem.Client;
using SchoolInformationSystem.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Kimlik do�rulama servislerini DI container'a ekle.
builder.Services.AddAuthorizationCore();

// HttpClient'a art�k ihtiya� duymad��� i�in bu kayd� basitle�tiriyoruz.
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthHttpHandler>();

// 2. API ile ileti�im kuracak HttpClient'� yap�land�r.
// Bu kay�t do�ru ve d�ng� olu�turmaz.
builder.Services.AddHttpClient("API", client =>
  //  client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    client.BaseAddress = new Uri("https://localhost:7289/"))
    .AddHttpMessageHandler<AuthHttpHandler>();

// 3. Bile�enlerin HttpClient'� kolayca enjekte edebilmesi i�in bu kayd� ekliyoruz.
// Bu sat�r hatan�n kayna��yd� ve �imdi d�ng� k�r�ld��� i�in do�ru �al��acak.
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

await builder.Build().RunAsync();