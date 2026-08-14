using Avae.Abstractions;
using Avae.Browser;
using Avae.Everywhere;
using Example.Razor;
using Example.Razor.Layout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    //DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
    //DefaultRequestVersion = HttpVersion.Version20,
    //Timeout = TimeSpan.FromSeconds(5)
});
builder.Services.UseSharedLibrary();
builder.Services.UseAvaeEssentials();
await builder.Services.UseEmbeddedAvaloniaApp("avalonia");
var app = builder.Build();
ServiceLocator.SetDefault(app.Services);
await app.RunAsync();
