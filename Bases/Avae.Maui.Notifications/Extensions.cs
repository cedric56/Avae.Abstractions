using Avae.Avalonia.Notifications;
using Avae.Services;
using Avalonia;
using Avalonia.Labs.Notifications;
using Application = Avalonia.Application;

namespace Avae.Maui.Notifications;

public static class Extensions
{
    private class EmbeddedApp : Application
    {

    }

    public static MauiAppBuilder WithAppNotifications(this MauiAppBuilder builder, AppNotificationOptions? options = null)
    {
#if ANDROID
        if (Android.App.Application.Context is null)
            throw new InvalidOperationException("Context must not be null");
#endif

        builder.Services.AddSingleton<ISystemNotificationService, SystemNotificationService>();

#if WINDOWS
        options ??= new AppNotificationOptions() { AppName = "Maui" };
        if (string.IsNullOrWhiteSpace(options.AppName))
            throw new InvalidOperationException($"{nameof(AppNotificationOptions)} {nameof(AppNotificationOptions.AppName)} must be declared");
#endif

        var appBuilder = AppBuilder.Configure<EmbeddedApp>()
            .WithAppNotifications(
#if ANDROID
            Android.App.Application.Context,
#endif
           options
           );

#if ANDROID
        appBuilder.UseAndroid();
#elif IOS || MACCATALYST
        appBuilder.UseiOS();
#else
        appBuilder.UsePlatformDetect();
#endif
        appBuilder.SetupWithoutStarting();
        return builder;
    }
}
