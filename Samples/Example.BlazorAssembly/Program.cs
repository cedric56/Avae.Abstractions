using Avae.Abstractions;
using Avae.DAL;
using Avae.Essentials;
using Avae.Notifications;
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
builder.Services.UseEssentials();
builder.Services.UseNotifications();
builder.Services.UseSharedLibrary(ServiceLifetime.Scoped);
await builder.Services.UseEmbeddedAvaloniaApp("avalonia", b => b.WithAppNotifications());
var app = builder.Build();
DBBase.Initialize(app.Services.GetRequiredService<IDBLayer>());
await app.RunAsync();