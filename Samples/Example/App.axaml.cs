using Avae.Avalonia;
using Avae.DAL;
using Avae.Essentials;
using Avae.Notifications;
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
using System;

namespace Example;

public partial class App(IServiceProvider provider) : Application
{
    public const string Icon = "avares://Example/Assets/avalonia-logo.ico";

    public static AppBuilder CreateApp(
        Action<IServiceCollection>? configureServices = null,
        Action<IServiceProvider>? afterBuildProvider = null,
         Action? onAppDispose = null)
    {
        return AvaeBuilder.CreateAvaloniaApp(
          Icon,
          true,
           sp => new App(sp),
           configureExternalServices: services =>
           {
               IconResolver.Register(new ExampleIconResolver());

               services.UseEssentials();
               services.UseNotifications();
               services.AddNavigationRegion("main"); 
               services.AddNavigationRegion("side");
               services.RegisterWithLifetime(HomeViewModel.TaskDialogKey, (sp, parameters) =>
               {
                   return parameters[0] switch
                   {
                       "Footer" => new TextBlock() { Text = "This is a footer" },
                       "IconSource" => new FABitmapIconSource() { UriSource = new Uri(Icon) },
                       "Content" => new TextBlock() { Text = "Here is content", FontSize = 27 },
                       _ => throw new NotImplementedException()
                   };
               });
               services.AddSingleton<MainViewModel>();
               services.Register<HomeView, HomeViewModel>();
               services.Register<MenuView, MenuViewModel>();
               services.Register<EssentialsView, EssentialsViewModel>();
               services.Register<RegionsView, RegionsViewModel>();
               services.RegisterWithLifetime<FormView, FormViewModel>();
               services.RegisterWithLifetime<FormPage1View, FormViewModel>(key: FormViewModel.KEY);
               services.RegisterWithLifetime<FormPage2View, FormPage2ViewModel>();
               services.RegisterWithLifetime<FormPage3View, FormPage3ViewModel, Person>((sp, person) => new FormPage3View(person));
               services.RegisterWithLifetime<ModalWindow, ModalViewModel>();
               configureServices?.Invoke(services);
           },
           afterBuild: afterBuildProvider,
           onDispose: 
           () =>
           {
               onAppDispose?.Invoke();
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        var mainView = new MainView()
        {
            DataContext = provider.GetRequiredService<MainViewModel>()
        };
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = mainView
            };
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
}
