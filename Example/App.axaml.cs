using Avae.Abstractions;
using Avae.Implementations;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using Example.ViewModels;
using Example.Views;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace Example;

public partial class App : AvaeApplication, IIocConfiguration
{
    public App()
        : base()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
    }

    public override void Configure(IIocContainer container)
    {
        container.Register<HomeView>();
        container.Register<MenuView>();
        container.Register<FormView>();
        container.Register<FormPage1View>();
        container.Register<FormPage2View>();
        container.Register<ModalWindow>();
    }

    public override void Configure(IServiceCollection services)
    {
        base.Configure(services);

        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<Router>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddTransient<ViewModelBaseFactory<FormViewModel>>();
        services.AddTransient<FormPage1ViewModel>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<ModalViewModel>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
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

        base.OnFrameworkInitializationCompleted();
    }
}
