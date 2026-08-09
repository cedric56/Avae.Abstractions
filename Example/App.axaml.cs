using Avae.Abstractions;
using Avae.Implementations;
using Avae.SignalR;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Example.Models;
using Example.ViewModels;
using Example.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ursa.Themes.Semi;

namespace Example;

public partial class App : AvaeApplication, IIocConfiguration
{
    ISignalRService? signalRService = null;
    Action? unsuscribe = null;

    public App()
        : base()
    {        
        //IconProvider.Current.Register<FontAwesomeIconProvider>();
    }

    public override string IconUrl => "avares://Example/Assets/avalonia-logo.ico";

    public override TypeDialog TypeDialog => TypeDialog.Fluent;

    protected override string Logs =>
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

        services.AddTransient<Router>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddTransient<ViewModelFactory<FormViewModel>>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<ViewModelFactory<FormPage3ViewModel>>();
        services.AddTransient<ModalViewModel>();

        if (!OperatingSystem.IsBrowser())
        {
            services.UseDBSqlLayer<SqliteConnection>(out signalRService, out unsuscribe);
            //services.UseDBOnionLayer(out signalRService, out unsuscribe);
        }
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        Styles.Add(new UrsaSemiTheme());
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

    public override void Dispose()
    {
        if (signalRService is not null)
        {
            _ = Task.Run(async () =>
            {
                await signalRService.StopAsync();
                await signalRService.DisposeAsync();
            });
        }

        unsuscribe?.Invoke();

        base.Dispose();
    }
}
