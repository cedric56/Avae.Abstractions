using Avae.DAL;
using Avae.Essentials;
using Avae.Notifications;
using Example.BlazorApp.Components;
using Example.Razor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Environment.WebRootPath)
});
builder.Services.UseBlazorEssentials();
builder.Services.UseNotifications();
//builder.Services.UseBlazorNotifications(
//    [
//        new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
//        {
//            Actions =
//            [
//                new("Hello", "hello"),
//                new("world", "world")
//            ]
//        }]
//    );
builder.Services.UseSharedLibrary(ServiceLifetime.Scoped, initialize: provider => CircuitServiceAccessor.Provider = provider);
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
var app = builder.Build();
DBBase.Initialize(app.Services.GetRequiredService<IDBLayer>());
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
