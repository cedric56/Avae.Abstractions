using Avae.Avalonia.Essentials;
using Avae.Avalonia.Notifications;
using Avae.Browser;
using Avae.Core;
using Avalonia.Labs.Notifications;
using Example.Razor;
using Example.Razor.Layout;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.UseAvaeEssentials();
builder.Services.UseAvaeNotifications();
builder.Services.UseSharedLibrary(true);
await builder.Services.UseEmbeddedAvaloniaApp("avalonia", b => b.WithAppNotifications());
var app = builder.Build();
ServiceLocator.SetDefault(app.Services);
await app.RunAsync();