using Avae.Maui.Services;
using Avae.Services;
using Avae.ViewModels;
using UXDivers.Popups.Maui;

namespace Avae.Maui;

/// <summary>
/// Extension methods for wiring up the Avae IoC container and its associated services
/// into a MAUI application's <see cref="MauiAppBuilder"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers a shared <see cref="IocContainer"/> instance (as <see cref="IIocContainer"/>,
    /// <see cref="IDialogService"/>, <see cref="IContentDialogService"/>, <see cref="ITaskDialogService"/>,
    /// <see cref="INotificationService"/>, and <see cref="IRequestedThemeService"/>), an <see cref="IIocContainer"/>,
    /// a transient <see cref="Router"/>, and a logger for <typeparamref name="TApp"/> with the MAUI service collection,
    /// and enables UXDivers popups support.
    /// </summary>
    /// <typeparam name="TApp">The application type the logger and container are configured for.</typeparam>
    /// <param name="builder">The MAUI app builder to configure.</param>
    /// <param name="configure">Optional callback invoked to register additional views/components with the IoC container.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    public static MauiAppBuilder ConfigureIocContainer<TApp>(this MauiAppBuilder builder,
        Action<IIocContainer>? configure = null)
        where TApp : Application
    {
        builder.UseUXDiversPopups();
        builder.Services.AddSingleton<IIocContainer>(sp =>
        {
            var container = new IocContainer(sp);
            configure?.Invoke(container);
            return container;
        });
        builder.Services.AddSingleton<IIocConfiguration>(sp => (IocContainer)sp.GetRequiredService<IIocContainer>());
        builder.Services.AddTransient<Router>();
        builder.Services.AddSingleton<Helper>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IContentDialogService, ContentDialogService>();
        builder.Services.AddSingleton<ITaskDialogService, TaskDialogService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IRequestedThemeService, RequestThemeService>();
        return builder;
    }
}