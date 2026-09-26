using Avae.Essentials;
using Avae.Notifications;
using Example.Razor;
using Microsoft.Extensions.Logging;

namespace Example.Hybrid;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .WithAppNotifications()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        builder.Services.RegisterEssentials();
        builder.Services.UseSharedLibrary(Avae.Razor.TypeRazorProject.Wasm);
        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        return app;
    }
}
