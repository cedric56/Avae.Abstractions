using Avae.Abstractions;
using Avae.DAL;
using Avae.Implementations;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Example.Dal;
using Example.ViewModels;
using Example.Views;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;
using System.Data.Common;
using System.Linq;

namespace Example;

public partial class App : AvaeApplication, IIocConfiguration
{
    public App()
        : base()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
    }

    public override string IconUrl => "avares://Example/Assets/avalonia-logo.ico";

    public override eTypeDialog TypeDialog => eTypeDialog.Box;

    public override void Configure(IIocContainer container)
    {        
        container.Register<HomeView>();
        container.Register<MenuView>();
        container.Register<FormViewModel>(new ViewFactory(parameters =>
        {
            if(parameters.OfType<FactoryParameter<string>>().Any(p => p.Value == "Page"))
            {
                return new FormPage1View();
            }
            return new FormView();
        }));
        container.Register<FormPage2View>();
        container.Register<FormPage3View>();
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

        services.AddTransient< DbConnection>(_ => new SqliteConnection("Data Source=data.db;Foreign Keys=True"));
        services.AddSingleton<IDataAccessLayer, DBBase>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private string GetCommandText(DbConnection connection)
    {
        if(connection is SqliteConnection sqlite)
        {
            return @"
            CREATE TABLE IF NOT EXISTS Person(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT,
                LastName TEXT
            );

            CREATE TABLE IF NOT EXISTS Contact(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            IdPerson INTEGER  NOT NULL,
                            IdContact INTEGER  NOT NULL,
                            CONSTRAINT FK_Contact_Person FOREIGN KEY(IdPerson) REFERENCES Person(Id),
                            CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY(IdContact) REFERENCES Person(Id)
                        );
            ";
        }
        else if(connection is SqlConnection sqlServer)
        {
            return @"CREATE TABLE IF NOT EXISTS Person (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        FirstName NVARCHAR(255) NULL,
                        LastName NVARCHAR(255) NULL,
                        Photo VARBINARY(MAX) NULL
                    );

                    CREATE TABLE IF NOT EXISTS Contact (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        IdPerson INT NOT NULL,
                        IdContact INT NOT NULL,
                        CONSTRAINT FK_Contact_Person FOREIGN KEY (IdPerson) REFERENCES Person(Id),
                        CONSTRAINT FK_Contact_ContactPerson FOREIGN KEY (IdContact) REFERENCES Person(Id)
                    );";
        }

        throw new NotImplementedException();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        using var connection = SimpleProvider.GetService<DbConnection>();
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = GetCommandText(connection);
        cmd.ExecuteNonQuery();

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
