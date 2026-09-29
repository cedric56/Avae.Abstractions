using Avae.Essentials;
using Example.Razor;
using Example.Razor.Layout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.UseBlazorEssentials();
//builder.Services.UseNotifications();
builder.Services.UseSharedLibrary(ServiceLifetime.Scoped);
//await builder.Services.UseEmbeddedAvaloniaApp("avalonia", b => b.WithAppNotifications());
var app = builder.Build();

var js = app.Services.GetRequiredService<IJSRuntime>();
await BrowserEssentials.InitializeAsync(js, true, "./BrowserEssentials.js");
var connectivity = app.Services.GetRequiredService<BlazorConnectivity>();
await connectivity.InitializeAsync("./BlazorEssentials.js");
await app.RunAsync();