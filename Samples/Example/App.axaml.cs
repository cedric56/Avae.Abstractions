using Avae.Avalonia;
using Avae.Avalonia.Essentials;
using Avae.Avalonia.Notifications;
using Avae.DAL;
using Avae.Services;
using Avae.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Example.DAL;
using Example.Models;
using Example.ViewModels;
using Example.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Optris.Icons.Avalonia;
using Optris.Icons.Avalonia.FontAwesome;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Example;

public partial class App : AvaeApplication
{
    public static AppBuilder Configure(
        Action<IServiceCollection>? configure = null)
    {
        return Configure<App>(configure: configure);
    }

    public static AppBuilder Configure<TApp>(
        Func<IServiceProvider, TApp>? factory = null,
        Action<IServiceCollection>? configure = null) where TApp : App
    {
        return AvaeBuilder
           .Configure<TApp>(
           sp => factory?.Invoke(sp) ?? (TApp)new App(sp),
           services =>
           {
               IconResolver.Register(new ExampleIconResolver());

               services.UseAvaeEssentials();
               services.UseAvaeNotifications();
               services.AddTransient<Router>();
               services.AddSingleton<HomeViewModel>();
               services.AddSingleton<MenuViewModel>();
               services.AddSingleton<EssentialsViewModel>();
               services.AddTransient<ViewModelFactory<FormViewModel>>();
               services.AddTransient<FormPage2ViewModel>();
               services.AddTransient<ViewModelFactory<FormPage3ViewModel>>();
               services.AddTransient<ModalViewModel>();

               if (!OperatingSystem.IsBrowser())
               {
                   //services.UseDBSqlLayer<SqliteConnection>();
                   services.UseDBOnionLayer();
               }

               if (OperatingSystem.IsWindows())
               {
                   services.AddSingleton<ILogger>(LoggerFactory.Create(b => b.AddDebug()).CreateLogger<App>());
               }

               configure?.Invoke(services);
           },
           container =>
           {
               container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
               {
                   return parameters[0] switch
                   {
                       "Footer" => new TextBlock() { Text = "This is a footer" },
                       "IconSource" => new FABitmapIconSource() { UriSource = new Uri("C:\\Users\\cedri\\source\\repos\\Avae.Abstractions\\Samples\\Example\\Assets\\avalonia-logo.ico") },
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
           });
    }

    protected IServiceProvider provider;

    public App(IServiceProvider provider) : base(provider)
    {
        this.provider = provider;

        DBBase.Initialize(provider.GetRequiredService<IDBLayer>());
    }

    Func<Task>? unsuscribe = null;

    public override string IconUrl => "avares://Example/Assets/avalonia-logo.ico";

    public override TypeDialog TypeDialog => TypeDialog.Fluent;

    protected string Logs =>
        Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Example"), "logs");

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
    }

    protected override Window GetMainWindow()
    {
        return new MainWindow();
    }

    protected override Control GetMainView()
    {
        return new MainView()
        {
            DataContext = new MainViewModel(new Router(provider))
        };
    }

    protected override async Task AfterCompletedAsync()
    {
        var monitor = provider.GetRequiredService<IDBMonitor<Person>>();

        Repository.Initialize(monitor);
        
        //unsuscribe = await provider.AddSignalR(monitor);
        unsuscribe = await monitor.AddStreamingHub();
    }

    public override async void Dispose()
    {
        if (unsuscribe != null)
            await unsuscribe();

        base.Dispose();
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
