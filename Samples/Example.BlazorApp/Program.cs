using Avae.Essentials;
using Avae.ViewModels;
using Example.BlazorApp.Components;
using Example.Razor;

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
    Runtime.BlazorServer,
    ServiceLifetime.Scoped,
    onCircuitProviderChanged: async provider => 
    await BlazorEssentials.InitializeAsync(provider, "./BlazorEssentials.js"));
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        // Increase to e.g. 100 MB. Adjust based on your needs.
        options.MaximumReceiveMessageSize = 100 * 1024 * 1024;
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