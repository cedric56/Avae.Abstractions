using Avae.Essentials;
using Example.BlazorApp.Components;
using Example.Razor;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Environment.WebRootPath)
});
builder.Services.UseBlazorEssentials();
//builder.Services.WithAppNotifications(new AppNotificationOptions()
//{
//    Channels = new[]
//    {
//        new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
//        {
//            Actions =
//            [
//                new("Hello", "hello"),
//                new("world", "world")
//            ]
//        }
//    }
//});
builder.Services.UseSharedLibrary(
    ServiceLifetime.Scoped,
     Avae.Razor.TypeRazorProject.Server,
    initialize: async provider =>
    {
        CircuitServiceAccessor.Provider = provider;
        var js = provider.GetRequiredService<IJSRuntime>();
        CircuitServiceAccessor.Runtime = js;
        //BrowserEssentials.SetModules(js, BrowserEssentials.InitializeAsync(js, false, "./BrowserEssentials.js"));
        await BrowserEssentials.InitializeAsync(js, false, "./BrowserEssentials.js");

        var connectivity = (BlazorConnectivity)provider.GetRequiredService<IConnectivity>();
        await connectivity.InitializeAsync("./BlazorEssentials.js");

        var appInfo = (BlazorAppInfo)provider.GetRequiredService<IAppInfo>();
        await appInfo.InitializeAsync("./BlazorEssentials.js");

        var filePicker = (BlazorFilePicker) provider.GetRequiredService<IFilePicker>();
        await filePicker.InitializeAsync("./BlazorEssentials.js");
    }
    );
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
var app = builder.Build();
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(Avae.Razor.Components.MainLayout).Assembly,
        typeof(Example.Razor.Components.Home).Assembly
    );
app.Run();