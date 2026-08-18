using Avae.Abstractions;
using Avae.Maui;
using Avalonia;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Example.Maui.Views;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Example.Maui
{
    public static class MauiProgram
    {
        private static void RegisterViews(IIocContainer container)
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
                    return new Label() { Text = "Hello " };
                }
                return new FormView();
            });
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .ConfigureIocContainer<App>(RegisterViews)
                .UseMauiApp<App>()
                //.UseMauiEssentials()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //builder.Services.UseAvaeEssentials();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddTransient<ModalViewModel>();
            builder.Services.AddTransient<FormViewModel>();
            builder.Services.UseDBSqlLayer<SqliteConnection>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceLocator.SetDefault(app.Services);
            return app;
        }
    }
}
