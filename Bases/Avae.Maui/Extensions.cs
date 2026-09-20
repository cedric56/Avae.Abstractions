using Avae.Services;
using Avae.ViewModels;

namespace Avae.Maui;

/// <summary>
/// Extension methods for wiring up the Avae IoC container and its associated services
/// into a MAUI application's <see cref="MauiAppBuilder"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers a shared 
    /// <see cref="IDialogService"/>, <see cref="IContentDialogService"/>, <see cref="ITaskDialogService"/>,
    /// <see cref="INotificationService"/>, and <see cref="IRequestedThemeService"/>),
    /// a transient <see cref="Router"/>,
    /// </summary>
    /// <param name="builder">The MAUI app builder to configure.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    public static MauiAppBuilder UseAvae(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<Router>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IContentDialogService, ContentDialogService>();
        builder.Services.AddSingleton<ITaskDialogService, TaskDialogService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IRequestedThemeService, RequestThemeService>();
        return builder;
    }
}