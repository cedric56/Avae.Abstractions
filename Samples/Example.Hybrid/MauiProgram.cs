using Avae.Core;
using Avae.Essentials.Core;
using Avae.Maui.Notifications;
using Example.Razor;
using Microsoft.Extensions.Logging;

namespace Example.Hybrid;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .WithSystemNotifications()
            .UseMauiApp<App>()               
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        builder.Services.RegisterEssentials();
        builder.Services.UseSharedLibrary();
        builder.Services.AddMauiBlazorWebView();            
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        ServiceLocator.SetDefault(app.Services);
        return app;
    }
}
