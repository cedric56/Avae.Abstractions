using Avae.Abstractions;
using Avae.Essentials;
using Example.Razor;
using Example.Razor.Layout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.UseSharedLibrary();
await builder.Services.UseEmbeddedAvaloniaApp();
var app = builder.Build();
ServiceLocator.SetDefault(app.Services);
await app.RunAsync();
