using Avae.Avalonia;
using Avae.Avalonia.Essentials;
using Avae.Avalonia.Notifications;
using Avae.DAL;
using Avae.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Example.DAL;
using Example.Models;
using Example.ViewModels;
using Example.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Example;

public partial class App : AvaeApplication, IIocConfiguration
{
    Func<Task>? unsuscribe = null;

    public override string IconUrl => "avares://Example/Assets/avalonia-logo.ico";

    public override TypeDialog TypeDialog => TypeDialog.Fluent;

    protected string Logs =>
        Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Example"), "logs");

    public override void Configure(IIocContainer container)
    {
        container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
        {
            return parameters[0] switch
            {
                "Footer" => new TextBlock() { Text = "This is a footer" },
                "IconSource" => new FABitmapIconSource() { UriSource = new Uri(IconUrl) },
                "Content" => new TextBlock() { Text = "Here is content", FontSize = 27 },
                _ => throw new NotImplementedException()
            };
        });
        container.Register<HomeView>();
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
        container.Register<ModalWindow>();
    }

    public override void Configure(IServiceCollection services)
    {
        base.Configure(services);

        services.UseAvaeNotifications();
        services.AddTransient<Router>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddSingleton<EssentialsViewModel>();
        services.AddTransient<ViewModelFactory<FormViewModel>>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<ViewModelFactory<FormPage3ViewModel>>();
        services.AddTransient<ModalViewModel>();
        services.UseAvaeEssentials();
        
        if (!OperatingSystem.IsBrowser())
        {
            services.UseDBSqlLayer<SqliteConnection>();
            //services.UseDBOnionLayer();
        }

        if(OperatingSystem.IsWindows())
        {
            services.AddSingleton<ILogger>(LoggerFactory.Create(b => b.AddDebug()).CreateLogger<App>());
        }
    }

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
            DataContext = new MainViewModel(new Router(Container.Provider))
        };
    }

    protected override async Task AfterCompletedAsync()
    {
        await base.AfterCompletedAsync();

        var monitor = Container.Provider.GetRequiredService<IDBMonitor<Person>>();

        //if (OperatingSystem.IsBrowser())
        //{
        //unsuscribe = await Container.Provider.AddSignalR(monitor);
        //}
        //else
        //{
        unsuscribe = await Container.Provider.AddSignalR(monitor);
        //unsuscribe = await Container.Provider.AddStreamingHub(monitor);
        //}
    }

    public override async void Dispose()
    {
        if (unsuscribe != null)
            await unsuscribe();

        base.Dispose();
    }
}
