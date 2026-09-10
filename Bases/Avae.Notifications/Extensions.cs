using Avae.Services;
using Avalonia;
using Avalonia.Labs.Notifications;
using Microsoft.JSInterop;
using Application = Avalonia.Application;

namespace Avae.Notifications;

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
#elif !BROWSER
        appBuilder.UsePlatformDetect();
#endif
        appBuilder.SetupWithoutStarting();
        return builder;
    }

    public static void UseAvaeNotifications(this IServiceCollection services)
    {
        services.AddSingleton<ISystemNotificationService, SystemNotificationService>();
    }

    public static void UseBlazorNotifications(this IServiceCollection services, IEnumerable<NotificationChannel>? channels = null)
    {
        services.AddScoped<ISystemNotificationService>(provider => new BlazorNotificationService(provider.GetRequiredService<IJSRuntime>(), channels));
    }
}
