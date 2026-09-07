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
using Dispatcher = Avalonia.Threading.Dispatcher;
using StyleInclude = Avalonia.Markup.Xaml.Styling.StyleInclude;

namespace Avae.Avalonia;

public enum TypeDialog
{
    Fluent,
    Box
}

public abstract class AvaeApplication : Application, IIocConfiguration, IDisposable, IRequestedThemeService
{
    public abstract string IconUrl { get; }
    public abstract TypeDialog TypeDialog {  get; }

    public AvaeApplication()
    {
        Container = new IocContainer(this);
    }

    public IocContainer Container { get; private set; }

    public virtual void Configure(IIocContainer container)
    {

    }

    public virtual void Configure(IServiceCollection services)
    {            
        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddSingleton<IIocConfiguration>(this);
        services.AddSingleton<IDialogService>(sp =>
        {
            return TypeDialog== TypeDialog.Box ? new DialogService(sp, IconUrl) :
                         sp.GetRequiredService<IContentDialogService>() as ContentDialogService ??
                         throw new InvalidOperationException("Failed to resolve IContentDialogService.");
        });
        services.AddTransient<INotificationService,NotificationService>();
        services.AddSingleton<IContentDialogService>(sp => new ContentDialogService(sp));
        services.AddSingleton<ITaskDialogService, TaskDialogService>();            
        services.AddSingleton<IRequestedThemeService>(this);
    }

    public void Configure(IServiceProvider provider)
    {
        ServiceLocator.SetDefault(provider);
    }

    public object? GetView(string key, params object[] @params)
    {
        return Container.GetView(key, @params);
    }

    public IViewFor? GetContextFor(string key, NavigableContext context)
    {
        return Container.GetView(key, [context]) as IViewFor;
    }

    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigableContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigableContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        return Container.GetModal<TViewModel, TResult>(context);
    }

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

    protected virtual Task AfterCompletedAsync()
    {
        return Task.CompletedTask;
    }

    protected abstract Window GetMainWindow();

    protected abstract Control GetMainView();

    public virtual void Dispose()
    {
        if (Container.Provider is IDisposable disposable)
            disposable.Dispose();

        GC.SuppressFinalize(this);
    }

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
