using Avae.Blazor.Essentials;
using Avae.Blazor.Notifications;
using Avae.Core;
using Example.BlazorApp;
using Example.BlazorApp.Components;
using Example.Razor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.Environment.WebRootPath)
});
builder.Services.UseBlazorEssentials();
builder.Services.UseBlazorNotifications();
builder.Services.UseSharedLibrary(true, extras: builder =>
{
    builder.OpenComponent<VideoCapture>(0);
    builder.CloseComponent();
});
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

var app = builder.Build();

ServiceLocator.SetDefault(app.Services);
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(Avae.Razor.Layout.MainLayout).Assembly,
        typeof(Example.Razor.Components.Home).Assembly
    );
app.UseServiceWorker();
app.Run();