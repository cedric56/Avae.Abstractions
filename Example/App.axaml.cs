using Autofac;
using Autofac.Core;
using Avae.Abstractions;
using Avae.Implementations;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Example.ViewModels;
using Example.Views;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System.Collections.Generic;
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

    public override bool IsStandard => true;

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
