using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Avalonia;

/// <summary>
/// Entry points for bootstrapping an Avalonia application together with its dependency injection
/// container and the standard Avae services (navigation, dialogs, notifications, theming).
/// </summary>
public static class AvaeBuilder
{
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
    public static AppBuilder CreateAvaloniaApp<TApp>(
       string icon,
       bool isFluent,
       Func<IServiceProvider, TApp> appFactory,
       Action<IServiceCollection>? configureExternalServices = null,
       Action<IIocContainer>? configureContainer = null,
       Action<IServiceProvider>? afterBuild = null,
       Action? onDispose = null) where TApp : Application
    {
        var services = new ServiceCollection();
        services.AddTransient<Router>();
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocContainer>(sp =>
        {
            var container = new IocContainer(sp);
            configureContainer?.Invoke(container);
            return container;
        });
        services.AddSingleton<IIocConfiguration>(sp => (IocContainer)sp.GetRequiredService<IIocContainer>());
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<IRequestedThemeService, RequestThemeService>();
        services.AddSingleton<IDialogService>(sp =>
        {
            if (isFluent)
                return (ContentDialogService)sp.GetRequiredService<IContentDialogService>();
            return new DialogService(sp, icon ?? string.Empty);
        });
        configureExternalServices?.Invoke(services);
        var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IIocContainer>();   // <- forces the factory, and configure, to run now
        afterBuild?.Invoke(provider);
        
        return AppBuilder
            .Configure(() => appFactory.Invoke(provider))
            .AfterSetup(builder =>
            {
                var app = builder.Instance;
                if (app != null)
                {
                    app.Styles.Add(new StyleInclude(provider)
                    {
                        Source = new Uri("avares://Avae.Avalonia/Modal/ModalStyle.axaml")
                    });
                    //Styles.Add(new FluentTheme());
                    app.Styles.Add(new FluentAvaloniaTheme());
                    app.Styles.Add(new ErrorStyle());

                    TopLevelStateManager.Initialize();
                    TopLevelStateManager.Default.ActiveChanged += OnActived;
                    
                    void OnActived(object? sender, EventArgs e)
                    {
                        TopLevelStateManager.Default.ActiveChanged -= OnActived;

                        var topLevel = TopLevelStateManager.Default.GetActive();
                        topLevel?.Closed += OnClosed;

                        void OnClosed(object? sender, EventArgs e)
                        {
                            topLevel?.Closed -= OnClosed;

                            onDispose?.Invoke();

                            if (provider is IDisposable disposable)
                                disposable.Dispose();
                        }
                    }
                }
            });
    }
}