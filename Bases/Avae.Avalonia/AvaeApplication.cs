using Avae.Core;
using Avae.Services;
using Avae.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Application = Avalonia.Application;
using StyleInclude = Avalonia.Markup.Xaml.Styling.StyleInclude;

namespace Avae.Avalonia;

/// <summary>
/// Specifies which dialog implementation an <see cref="AvaeApplication"/> should use.
/// </summary>
public enum TypeDialog
{
    /// <summary>
    /// Use FluentAvalonia's content dialog service.
    /// </summary>
    Fluent,

    /// <summary>
    /// Use a simple boxed dialog implementation.
    /// </summary>
    Box
}

/// <summary>
/// Base class for the Avalonia application entry point, wiring up dependency injection, navigation,
/// dialog services, and framework lifecycle handling for Avae-based applications.
/// </summary>
public abstract class AvaeApplication : Application, IIocConfiguration, IDisposable, IRequestedThemeService
{
    /// <summary>
    /// Gets the URL of the application's icon, used by the boxed dialog service.
    /// </summary>
    public abstract string IconUrl { get; }

    /// <summary>
    /// Gets which dialog implementation this application uses.
    /// </summary>
    public abstract TypeDialog TypeDialog { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaeApplication"/> class, creating its <see cref="IocContainer"/>.
    /// </summary>
    public AvaeApplication()
    {
        Container = new IocContainer(this);
    }

    /// <summary>
    /// Gets the dependency injection / view resolution container for this application.
    /// </summary>
    public IocContainer Container { get; private set; }

    /// <summary>
    /// When overridden, registers views and other components with the IoC container.
    /// The base implementation does nothing.
    /// </summary>
    /// <param name="container">The container to configure.</param>
    public virtual void Configure(IIocContainer container)
    {

    }

    /// <summary>
    /// Registers the application's default services—broker, dialog, notification, and theme
    /// services—with the dependency injection container. Derived classes should call the base
    /// implementation when overriding to retain these registrations.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public virtual void Configure(IServiceCollection services)
    {
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocConfiguration>(this);
        services.AddSingleton<IDialogService>(sp =>
        {
            return TypeDialog == TypeDialog.Box ? new DialogService(sp, IconUrl) :
                         sp.GetRequiredService<IContentDialogService>() as ContentDialogService ??
                         throw new InvalidOperationException("Failed to resolve IContentDialogService.");
        });
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<IRequestedThemeService>(this);
    }

    /// <summary>
    /// Registers the built service provider with <see cref="ServiceLocator"/> as the application-wide default.
    /// </summary>
    /// <param name="provider">The built service provider.</param>
    public void Configure(IServiceProvider provider)
    {
        ServiceLocator.SetDefault(provider);
    }

    /// <summary>
    /// Resolves and creates the view registered under the specified key.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="params">Additional arguments passed through to the view's registered factory.</param>
    /// <returns>The created view instance, or <see langword="null"/> if resolution fails.</returns>
    public object? GetView(string key, params object[] @params)
    {
        return Container.GetView(key, @params);
    }

    /// <summary>
    /// Resolves the view registered under the specified key, using the supplied navigation context.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor"/>, or <see langword="null"/> if resolution fails or the result is not an <see cref="IViewFor"/>.</returns>
    public IViewFor? GetContextFor(string key, NavigableContext context)
    {
        return Container.GetView(key, [context]) as IViewFor;
    }

    /// <summary>
    /// Resolves the strongly typed view for the specified view model type, using the supplied navigation context.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type whose view should be resolved.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor{TViewModel}"/>, or <see langword="null"/> if resolution fails or the result does not match.</returns>
    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigableContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    /// <summary>
    /// Resolves the modal view associated with the specified closeable view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type whose modal view should be resolved.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved modal view, or <see langword="null"/> if resolution fails.</returns>
    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigableContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        return Container.GetModal<TViewModel, TResult>(context);
    }

    /// <summary>
    /// Completes application startup: registers global styles, initializes top-level state management,
    /// creates and assigns the main view or window depending on the current application lifetime, and
    /// wires up disposal when the main view is closed.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        Styles.Add(new StyleInclude(Container.Provider)
        {
            Source = new Uri("avares://Avae.Avalonia/Modal/ModalStyle.axaml")
        });
        //Styles.Add(new FluentTheme());
        Styles.Add(new FluentAvaloniaTheme());
        Styles.Add(new ErrorStyle());

        TopLevelStateManager.Initialize();

        var mainView = GetMainView();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = GetMainWindow();
            window.Content = mainView;
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
        {
            activityLifetime.MainViewFactory = () => mainView;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = mainView;

        }

        base.OnFrameworkInitializationCompleted();

        _ = Task.Run(AfterCompletedAsync);

        mainView.Loaded += OnLoaded;
        void OnLoaded(object? sender, RoutedEventArgs e)
        {
            mainView.Loaded -= OnLoaded;

            var topLevel = TopLevel.GetTopLevel(mainView);
            topLevel?.Closed += OnClosed;

            void OnClosed(object? sender, EventArgs e)
            {
                topLevel?.Closed -= OnClosed;
                Dispose();
            }
        }
    }

    /// <summary>
    /// When overridden, runs additional asynchronous startup logic after framework initialization completes.
    /// Runs on a background thread and is not awaited by <see cref="OnFrameworkInitializationCompleted"/>.
    /// The base implementation does nothing.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task AfterCompletedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// When implemented in a derived class, creates the application's main window.
    /// </summary>
    /// <returns>The main window instance.</returns>
    protected abstract Window GetMainWindow();

    /// <summary>
    /// When implemented in a derived class, creates the application's main view.
    /// </summary>
    /// <returns>The main view control.</returns>
    protected abstract Control GetMainView();

    /// <summary>
    /// Disposes the underlying service provider, if it implements <see cref="IDisposable"/>.
    /// </summary>
    public virtual void Dispose()
    {
        if (Container.Provider is IDisposable disposable)
            disposable.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Applies the specified theme to the application by setting <see cref="Application.RequestedThemeVariant"/>.
    /// </summary>
    /// <param name="theme">The requested theme.</param>
    public void Request(RequestedTheme theme)
    {
        RequestedThemeVariant = theme switch
        {
            RequestedTheme.Light => ThemeVariant.Light,
            RequestedTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }
}