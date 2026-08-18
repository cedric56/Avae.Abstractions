using Avae.Services;
using Avalonia;
using Avalonia.Labs.Notifications;

namespace Avae.Maui.Notifications;

public static class Extensions
{
    private class App : Avalonia.Application
    {

    }

    public static MauiAppBuilder WithSystemNotifications(this MauiAppBuilder builder, AppNotificationOptions? options = null)
    {
        builder.Services.AddSingleton<ISystemNotificationService, SystemNotificationService>();

#if WINDOWS
        options ??= new AppNotificationOptions() { AppName = "Maui" };
        if (string.IsNullOrWhiteSpace(options.AppName))
            throw new InvalidOperationException($"{nameof(AppNotificationOptions)} {nameof(AppNotificationOptions.AppName)} must be declared");
#endif

        var appBuilder = AppBuilder.Configure<App>()
            .WithAppNotifications(
#if ANDROID
            Android.App.Application.Context,
#endif
           options
           );
#if ANDROID

#else
        appBuilder.UsePlatformDetect();
#endif
        appBuilder.SetupWithoutStarting();
        return builder;
    }
}
