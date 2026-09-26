using Avae.DAL;
using Avae.Essentials;
using Avae.Maui;
using Avae.ViewModels;
using CommunityToolkit.Maui;
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
//            .WithAppNotifications(new AppNotificationOptions()
//            {
//#if WINDOWS
//                AppIcon = Path.Combine(AppContext.BaseDirectory, "appicon.ico"),
//                AppName = "Maui example"
//#endif
//            })
            .UseAvae()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseFontAwesomeSolidMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.RegisterEssentials();
        builder.Services.RegisterWithLifetime(HomeViewModel.TaskDialogKey, (sp, args) =>
        {
            return args[0] switch
            {
                "Footer" => new Label() { Text = "This is a footer" },
                "IconSource" => new UriImageSource { Uri = new Uri(Path.Combine(AppContext.BaseDirectory, "appicon.ico")) },
                "Content" => new Label() { Text = "Here is content", FontSize = 27 },
                _ => throw new NotImplementedException()
            };
        });
        builder.Services.AddNavigationRegion("main");
        builder.Services.AddNavigationRegion("side");
        builder.Services.AddSingleton<MainViewModel>();
        //builder.Services.Register<MainPage, MainViewModel>();
        builder.Services.Register<HomeView ,HomeViewModel>();
        builder.Services.Register<MenuView, MenuViewModel>();
        builder.Services.Register<DefaultPage, RegionsViewModel>();
        builder.Services.Register<EssentialsView, EssentialsViewModel>();
        builder.Services.RegisterWithLifetime<ModalView, ModalViewModel>();
        builder.Services.RegisterWithLifetime<FormView, FormViewModel>();
        builder.Services.RegisterWithLifetime<DefaultView, FormViewModel>(key: FormViewModel.KEY);
        builder.Services.AddPersonServiceLocal<SqliteConnection>();
        builder.Services.AddSingleton<IDBMonitor<Person>, DBMonitor<Person>>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        IconResolver.Register(new ExampleIconResolver());
        return app;
    }
}

class DefaultView : ContentView, IViewFor
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

class DefaultPage : ContentPage, IViewFor
{
    public object? Context
    {
        get => BindingContext;
        set
        {
            BindingContext = value;
            if (value is RegionsViewModel viewModel)
                this.Content = new Label() { Text = "Regions" };
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