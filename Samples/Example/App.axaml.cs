using Avae.Avalonia;
using Avae.DAL;
using Avae.Essentials;
using Avae.Notifications;
using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Example.Models;
using Example.ViewModels;
using Example.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Optris.Icons.Avalonia;
using Optris.Icons.Avalonia.FontAwesome;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Example;

public partial class App(IServiceProvider provider) : Application
{
    public const string Icon = "avares://Example/Assets/avalonia-logo.ico";

    public static AppBuilder CreateApp(
        Action<IServiceCollection>? configureServices = null,
        Action<IServiceProvider>? afterBuildProvider = null,
         Action? onAppDispose = null)
    {
        Func<Task> unsuscribe = () => Task.CompletedTask;
        return AvaeBuilder.CreateAvaloniaApp(
          Icon,
          true,
           sp => new App(sp),
           configureExternalServices: services => ConfigureServices(services, configureServices),
           configureContainer: container => ConfigureContainer(container),
           afterBuild: sp => AfterBuild(sp, afterBuildProvider),
           onDispose: async () =>
           {
               await unsuscribe.Invoke();
               onAppDispose?.Invoke();
           });
    }

    private static async void AfterBuild(IServiceProvider provider, Action<IServiceProvider>? afterBuildProvider)
    {
        DBBase.Initialize(provider.GetRequiredService<IDBLayer>());

        var monitor = provider.GetRequiredService<IDBMonitor<Person>>();

        Repository.Initialize(monitor);

        var http = provider.GetService<HttpMessageHandler>();

        //unsuscribe = await monitor.AddStreamingHub(http);

        Func<HttpMessageHandler, HttpMessageHandler> factory = null!;
        if (http != null)
            factory = _ => http;

        //unsuscribe = await monitor.AddSignalR(factory: factory);

        afterBuildProvider?.Invoke(provider);
    }

    private static void ConfigureContainer(IIocContainer container)
    {
        container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
        {
            return parameters[0] switch
            {
                "Footer" => new TextBlock() { Text = "This is a footer" },
                "IconSource" => new FABitmapIconSource() { UriSource = new Uri(Icon) },
                "Content" => new TextBlock() { Text = "Here is content", FontSize = 27 },
                _ => throw new NotImplementedException()
            };
        });
        container.Register<HomeView>((sp, context) => new HomeView(sp.GetRequiredService<IDialogService>()));
        container.Register<MenuView>();
        container.Register<EssentialsView>();
        container.Register<FormViewModel>((sp, context) =>
        {
            if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
            {
                return new FormPage1View();
            }
            return new FormView();
        });
        container.Register<FormPage2View>();
        container.Register<FormPage3View, Person>((sp, person) => new FormPage3View(person));
        container.Register<ModalWindow>((sp, context) => new ModalWindow(sp.GetRequiredService<IContentDialogService>()));
    }

    private static void ConfigureServices(IServiceCollection services, Action<IServiceCollection>? configureServices = null)
    {
        IconResolver.Register(new ExampleIconResolver());

        services.UseEssentials();
        services.UseAvaeNotifications();
        services.AddTransient<Router>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddSingleton<EssentialsViewModel>();
        services.AddTransient<ViewModelFactory<FormViewModel>>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<ViewModelFactory<FormPage3ViewModel>>();
        services.AddTransient<ModalViewModel>();
        configureServices?.Invoke(services);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        var mainView = new MainView()
        {
            DataContext = new MainViewModel(new Router(provider))
        };
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
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
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    class ExampleIconResolver : IIconResolver
    {
        static ExampleIconResolver()
        {
            IconProvider.Current.Register<FontAwesomeIconProvider>();
        }

        public object? GetIcon(string key)
        {
            return new Icon() { Value = key };
        }

        public object? GetSource(string key)
        {
            throw new NotImplementedException();
        }
    }
}
