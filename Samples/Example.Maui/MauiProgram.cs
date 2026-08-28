using Avae.Core;
using Avae.Essentials;
using Avae.Maui;
using Avae.Maui.Notifications;
using Avae.ViewModels;
using Avalonia.Labs.Notifications;
using Example.DAL;
using Example.Maui.Views;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Example.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .WithAppNotifications(new AppNotificationOptions()
            {
#if WINDOWS
                AppIcon = "C:\\Users\\cedri\\source\\repos\\Avae.Abstractions\\Samples\\Example.Maui\\Resources\\Images\\dotnet_bot.png",
                AppName = "Maui example"
#endif
            })
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureIocContainer<App>(container =>
            {
                container.Register(HomeViewModel.TaskDialogKey, (sp, parameters) =>
                {
                    return parameters[0] switch
                    {
                        "Footer" => new Label() { Text = "This is a footer" },
                        "IconSource" => ImageSource.FromFile("dotnet_bot.png"),
                        "Content" => new Label() { Text = "Here is content", FontSize = 27 },
                        _ => throw new NotImplementedException()
                    };
                });
                container.Register<MainPage>();
                container.Register<HomeView>();
                container.Register<MenuView>();
                container.Register<ModalView>();
                container.Register<EssentialsView>();
                //container.Register<FormView>();
                container.Register<FormViewModel>((sp, context) =>
                {
                    if (context.FactoryParameters.OfType<string>().Any(p => p == FormViewModel.KEY))
                    {
                        return new DefaultView();
                    }
                    return new FormView();
                });
            });

        builder.Services.RegisterEssentials();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MenuViewModel>();
        builder.Services.AddTransient<EssentialsViewModel>();
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

class DefaultView : ContentView, IViewFor<FormViewModel>
{
    public object? Context
    {
        get => BindingContext; 
        set
        {
            BindingContext = value;
            if (value is FormViewModel viewModel)
                this.Content = new Label() { Text = "Form"};
        }
    }
}
