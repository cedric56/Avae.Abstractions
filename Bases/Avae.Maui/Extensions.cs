using Avae.Services;
using Avae.ViewModels;
using Microsoft.Extensions.Logging;
using UXDivers.Popups.Maui;

namespace Avae.Maui;

/// <summary>
/// Extension methods for wiring up the Avae IoC container and its associated services
/// into a MAUI application's <see cref="MauiAppBuilder"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers a shared <see cref="IocConfiguration"/> instance (as <see cref="IIocConfiguration"/>,
    /// <see cref="IDialogService"/>, <see cref="IContentDialogService"/>, <see cref="ITaskDialogService"/>,
    /// <see cref="INotificationService"/>, and <see cref="IRequestedThemeService"/>), an <see cref="IIocContainer"/>,
    /// a transient <see cref="Router"/>, and a logger for <typeparamref name="TApp"/> with the MAUI service collection,
    /// and enables UXDivers popups support.
    /// </summary>
    /// <typeparam name="TApp">The application type the logger and container are configured for.</typeparam>
    /// <param name="builder">The MAUI app builder to configure.</param>
    /// <param name="configure">Optional callback invoked to register additional views/components with the IoC container.</param>
    /// <param name="build">Optional callback used to configure logging providers.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    public static MauiAppBuilder ConfigureIocContainer<TApp>(this MauiAppBuilder builder,
        Action<IIocContainer>? configure = null,
        Action<ILoggingBuilder>? build = null)
        where TApp : Application
    {
        builder.UseUXDiversPopups();
        builder.Services.AddSingleton<IIocContainer>(sp => new IocContainer(sp, GetConfiguration(sp)));
        builder.Services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, () => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
        builder.Services.AddTransient<Router>(sp => new Router(sp));
        builder.Services.AddSingleton<IDialogService>(GetConfiguration);
        builder.Services.AddSingleton<IContentDialogService>(GetConfiguration);
        builder.Services.AddSingleton<ITaskDialogService>(GetConfiguration);
        builder.Services.AddSingleton<INotificationService>(GetConfiguration);
        builder.Services.AddSingleton<IRequestedThemeService>(GetConfiguration);
        builder.Services.AddSingleton<ILogger>(LoggerFactory.Create(builder => build?.Invoke(builder)).CreateLogger<TApp>());
        return builder;

        IocConfiguration GetConfiguration(IServiceProvider provider)
        {
            return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
        }
    }
}