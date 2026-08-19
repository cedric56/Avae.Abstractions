using Avae.ViewModels;
using Avae.Core;
using Avae.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using Microsoft.Extensions.DependencyInjection;


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
        services.AddSingleton<ISystemNotificationService, SystemNotificationService>();
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

    public IViewFor? GetContextFor(string key, NavigationContext context)
    {
        return Container.GetView(key, [context]) as IViewFor;
    }

    /// <summary>
    /// Obtain the view by the viewModel association
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="params"></param>
    /// <returns></returns>
    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    /// <summary>
    /// Obtain the view by the modalViewModel association
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="params"></param>
    /// <returns></returns>
    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        var view = Container.GetView(typeof(TViewModel).Name, [context]);

        // Now verify the TResult type matches
        Type viewType = view.GetType();
        Type[] interfaces = viewType.GetInterfaces();

        // Find the IModalFor<,> interface implementation
        var modalInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

        if (modalInterface != null)
        {
            Type[] genericArgs = modalInterface.GetGenericArguments();
            Type viewModelType = genericArgs[0];
            Type resultType = genericArgs[1];

            // Check if the TResult matches
            if (resultType != typeof(TResult))
            {
                throw new InvalidOperationException(
                    $"The view associated with view model {typeof(TViewModel).Name} expects result type {resultType.Name}, " +
                    $"but {typeof(TResult).Name} was requested.");
            }
        }

        return view as IModalFor<TViewModel, TResult> ?? throw new InvalidOperationException($"The view associated with the view model {typeof(TViewModel).Name} is not a modal view.");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Styles.Add(new StyleInclude(Container.Provider)
        {
            Source = new Uri("avares://Avae.Avalonia/Modal/ModalStyle.axaml")
        });
        //Styles.Add(new FluentTheme());
        Styles.Add(new FluentAvaloniaTheme());

        TopLevelStateManager.Initialize();

        var mainView = GetMainView();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = GetMainWindow();
            window.Content = mainView;
            desktop.MainWindow = window;
            desktop.Exit += OnDesktopExit;
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

        void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            desktop?.Exit -= OnDesktopExit;
            Dispose();
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
        Application.Current?.RequestedThemeVariant = theme switch
        {
            RequestedTheme.Light => ThemeVariant.Light,
            RequestedTheme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }
}
