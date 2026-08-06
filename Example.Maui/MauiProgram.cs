using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Maui;
using Avae.SignalR;
using Example.Maui.Views;
using Example.Models;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Example.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .ConfigureIocContainer<App>(container =>
                {
                    container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
                    {
                        return parameters[0] switch
                        {
                            "Footer" => new Label() { Text = "This is a footer" },
                            "IconSource" => ImageSource.FromUri(new Uri("C:\\Users\\cedri\\source\\repos\\Avae.Abstractions\\Example.Maui\\Resources\\AppIcon\\appicon.svg")),
                            "Content" => new Label() { Text = "Here is content", FontSize = 27 },
                            _ => throw new NotImplementedException()
                        };
                    });
                    container.Register<MainPage>();
                    container.Register<HomeView>();
                    container.Register<MenuView>();
                    container.Register<ModalView>();
                    //container.Register<FormView>();
                    container.Register<FormViewModel>((sp, context) =>
                    {
                        if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
                        {
                            return new Label() { Text = "Hello "};
                        }
                        return new FormView();
                    });
                })
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddTransient<ModalViewModel>();
            builder.Services.AddTransient<FormViewModel>();

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";

            builder.Services.UseDbLayer<IDBLayer>(sp => new DBSqlLayer(sp));
            builder.Services.UseSqlMonitors<SqliteConnection>(connectionString, (factory) =>
            {
                var monitor = factory.AddDbMonitor<Person>();
                var e= monitor.AddSignalR("http://localhost:5001/PersonHub", out _);
                builder.Services.AddSingleton<ISqlMonitor<Person>>(monitor);

            }, true);
            //builder.Services.AddTransient<IDbConnection>(sp =>
            //{
            //    return new SqliteConnection(connectionString);
            //});
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceLocator.SetDefault(app.Services);
            return app;
        }
    }
}
