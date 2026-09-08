using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Avalonia;

/// <summary>
/// Entry points for bootstrapping an Avalonia application together with its dependency injection
/// container and the standard Avae services (navigation, dialogs, notifications, theming).
/// </summary>
public static class AvaeBuilder
{
    /// <summary>
    /// Configures an <see cref="AppBuilder"/> for a plain <see cref="Application"/>-derived type,
    /// constructing the app instance from the built service provider and registering the standard
    /// Avae services.
    /// </summary>
    /// <typeparam name="TApp">The application type to configure.</typeparam>
    /// <param name="appFactory">A factory that creates the application instance given the built service provider.</param>
    /// <param name="iconUrl">The icon URL used by the boxed dialog service when <paramref name="typeDialog"/> is <see cref="TypeDialog.Box"/>.</param>
    /// <param name="configureServices">Optional callback to register additional services with the container.</param>
    /// <param name="configure">Optional callback to register additional views/components with the IoC container.</param>
    /// <param name="typeDialog">Which dialog implementation to use. Defaults to <see cref="TypeDialog.Box"/>.</param>
    /// <returns>The configured <see cref="AppBuilder"/>, ready for further setup (e.g. platform detection, lifetime).</returns>
    public static AppBuilder CreateAvaloniaApp<TApp>(
       Func<IServiceProvider, TApp> appFactory,
       string iconUrl,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       TypeDialog typeDialog = TypeDialog.Box) where TApp : Application
    {
        IServiceProvider? provider = null;
        return AppBuilder
            .Configure(() => appFactory(provider!))
            .ConfigureIocContainer<TApp>(
            out provider,
            services => configureServices?.Invoke(services),
            container => configure?.Invoke(container),
            iconUrl,
            typeDialog);
    }

    /// <summary>
    /// Configures an <see cref="AppBuilder"/> for an <see cref="AvaeApplication"/>-derived type,
    /// constructing the app instance from the built service provider and registering the standard
    /// Avae services. The application's own <see cref="AvaeApplication.IconUrl"/> and
    /// <see cref="AvaeApplication.TypeDialog"/> are used for dialog configuration.
    /// </summary>
    /// <typeparam name="TApp">The <see cref="AvaeApplication"/>-derived application type to configure.</typeparam>
    /// <param name="appFactory">A factory that creates the application instance given the built service provider.</param>
    /// <param name="configureServices">Optional callback to register additional services with the container.</param>
    /// <param name="configure">Optional callback to register additional views/components with the IoC container.</param>
    /// <returns>The configured <see cref="AppBuilder"/>, ready for further setup (e.g. platform detection, lifetime).</returns>
    public static AppBuilder CreateAvaeApp<TApp>(
       Func<IServiceProvider, TApp> appFactory,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null) where TApp : AvaeApplication
    {
        IServiceProvider? provider = null;
        return AppBuilder
            .Configure(() => appFactory(provider!))
            .ConfigureIocContainer<TApp>(
            out provider,
            services => configureServices?.Invoke(services),
            container => configure?.Invoke(container));
    }

    /// <summary>
    /// Builds a service provider registering the application instance, router, IoC container, and
    /// the standard Avae services (broker, notification, content/task/boxed dialog services, and
    /// theme service), then invokes <paramref name="configureServices"/> for any additional registrations.
    /// </summary>
    /// <typeparam name="TApp">The application type being configured, resolved from <c>builder.Instance</c>.</typeparam>
    /// <param name="builder">The app builder whose <c>Instance</c> supplies the constructed application.</param>
    /// <param name="provider">When this method returns, the service provider built from the configured services.</param>
    /// <param name="configureServices">Optional callback to register additional services with the container.</param>
    /// <param name="configure">Optional callback invoked when the <see cref="IIocContainer"/> is created, to register additional views/components.</param>
    /// <param name="iconUrl">
    /// The icon URL used by the boxed dialog service. Overridden by <see cref="AvaeApplication.IconUrl"/>
    /// if <typeparamref name="TApp"/> is an <see cref="AvaeApplication"/>.
    /// </param>
    /// <param name="typeDialog">
    /// Which dialog implementation to use. Overridden by <see cref="AvaeApplication.TypeDialog"/>
    /// if <typeparamref name="TApp"/> is an <see cref="AvaeApplication"/>. Defaults to <see cref="TypeDialog.Box"/>.
    /// </param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    public static AppBuilder ConfigureIocContainer<TApp>(this AppBuilder builder,
       out IServiceProvider provider,
       Action<IServiceCollection>? configureServices = null,
       Action<IIocContainer>? configure = null,
       string? iconUrl = null,
       TypeDialog typeDialog = TypeDialog.Box)
       where TApp : Application
    {
        var services = new ServiceCollection();
        services.AddSingleton<TApp>(_ => (TApp)builder.Instance!);
        services.AddTransient<Router>(sp => new Router(sp));
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocContainer>(sp =>
        {
            var container = new IocContainer(sp);
            configure?.Invoke(container);
            return container;
        });
        services.AddSingleton<IIocConfiguration>(sp => (IocContainer)sp.GetRequiredService<IIocContainer>());
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<IRequestedThemeService, RequestThemeService>();
        services.AddSingleton<IDialogService>(sp =>
        {
            var app = sp.GetRequiredService<TApp>();
            if (app is AvaeApplication avae)
            {
                iconUrl = avae.IconUrl;
                typeDialog = avae.TypeDialog;
            }

            if (typeDialog == TypeDialog.Fluent)
                return (ContentDialogService)sp.GetRequiredService<IContentDialogService>();
            return new DialogService(sp, iconUrl ?? string.Empty);
        });
        configureServices?.Invoke(services);
        provider = services.BuildServiceProvider();
        return builder;
    }
}