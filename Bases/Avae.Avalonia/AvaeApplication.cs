using Avae.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using FluentAvalonia.Styling;
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
public abstract class AvaeApplication(IServiceProvider provider) : Application, 
    IDisposable, IRequestedThemeService
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
    /// Completes application startup: registers global styles, initializes top-level state management,
    /// creates and assigns the main view or window depending on the current application lifetime, and
    /// wires up disposal when the main view is closed.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        Styles.Add(new StyleInclude(provider)
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
        if (provider is IDisposable disposable)
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