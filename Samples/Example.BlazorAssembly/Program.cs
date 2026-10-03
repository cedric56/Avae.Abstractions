using Avae.Essentials;
using Avae.ViewModels;
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
builder.Services.UseBlazorEssentials();
//builder.Services.UseNotifications();
builder.Services.UseSharedLibrary(Runtime.BlazorWebAssembly);
//await builder.Services.UseEmbeddedAvaloniaApp("avalonia", b => b.WithAppNotifications());
var app = builder.Build();
await BlazorEssentials.InitializeAsync(app.Services);
await app.RunAsync();