using Avae.DAL;
using Avae.Essentials;
using Avae.Maui;
using Avae.Notifications;
using Avae.ViewModels;
using Avalonia.Labs.Notifications;
using CommunityToolkit.Maui;
using Example.DAL;
using Example.Maui.Views;
using Example.Models;
using Example.ViewModels;
using MauiIcons.Core;
using MauiIcons.FontAwesome.Solid;
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
                AppIcon = Path.Combine(AppContext.BaseDirectory, "appicon.ico"),
                AppName = "Maui example"
#endif
            })
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseFontAwesomeSolidMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseAvae();
        builder.Services.UseNotifications();
        builder.Services.RegisterEssentials();
        builder.Services.Register<MainPage, MainViewModel>();
        builder.Services.Register<HomeView ,HomeViewModel>();
        builder.Services.Register<MenuView, MenuViewModel>();
        builder.Services.Register<EssentialsView, EssentialsViewModel>();
        builder.Services.RegisterWithLifetime<ModalView, ModalViewModel>(ServiceLifetime.Transient);
        builder.Services.Register<FormView, FormViewModel>();
        builder.Services.Register<DefaultView, FormViewModel>(key: FormViewModel.KEY);
        builder.Services.UseDBSqlLayer<SqliteConnection>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        IconResolver.Register(new ExampleIconResolver());
        Repository.Initialize(app.Services.GetRequiredService<IDBMonitor<Person>>());
        DBBase.Initialize(app.Services.GetRequiredService<IDBLayer>());
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
                this.Content = new Label() { Text = "Form" };
        }
    }
}

class ExampleIconResolver : IIconResolver
{
    public object? GetIcon(string path)
    {
        if (path.StartsWith("fa-solid fa-"))
        {
            var name = path["fa-solid fa-".Length..].Replace("-", "");
            if (Enum.TryParse<FontAwesomeSolidIcons>(name, true, out var icon))
            {
                return icon;
            }
        }
        return null;
    }

    public object? GetSource(string key)
    {
        if (key.StartsWith("fa-solid fa-"))
        {
            var name = key["fa-solid fa-".Length..].Replace("-", "");
            if (Enum.TryParse<FontAwesomeSolidIcons>(name, true, out var icon))
            {
                return icon.ToImageSource();
            }
        }
        return null;
    }
}