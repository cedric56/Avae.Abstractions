using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Implementations;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Example.Models;
using Example.ViewModels;
using Example.Views;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.IO;
using System.Linq;
using Ursa.Themes.Semi;

namespace Example;

public partial class App : AvaeApplication, IIocConfiguration
{
    public App()
        : base()
    {        
        IconProvider.Current.Register<FontAwesomeIconProvider>();
    }

    public override string IconUrl => "avares://Example/Assets/avalonia-logo.ico";

    public override TypeDialog TypeDialog => TypeDialog.Box;

    public override void Configure(IIocContainer container)
    {
        container.Register<HomeView>();
        container.Register<MenuView>();
        container.Register<FormViewModel>(parameters =>
        {
            if (parameters.OfType<FactoryParameter<string>>().Any(p => p.Value == FormViewModel.KEY))
            {
                return new FormPage1View();
            }
            return new FormView();
        });
        container.Register<FormPage2View>();
        container.Register<FormPage3View, Person>(person => new FormPage3View(person));
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

        services.UseDbLayer<IDBLayer, DBSqlLayer>();

        if (!OperatingSystem.IsBrowser())
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";
            services.UseSqlMonitors<SqliteConnection>(connectionString, factory =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                monitor.AddSignalR("http://localhost:5001/PersonHub");
                services.AddSingleton<ISqlMonitor<Person>>(monitor);
            });
        }
        services.AddSingleton<IBrokerService, BrokerService>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        Styles.Add(new SemiTheme());

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        //BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(new Router())
            };
        }
        else if(ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView()
            {
                DataContext = new MainViewModel(new Router())
            };
        }
    }
}
