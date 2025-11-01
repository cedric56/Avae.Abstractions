using Avae.Abstractions;
using Avae.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using Example.ViewModels;
using Example.Views;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;

namespace Example;

public partial class App : Application, IIocConfiguration
{
    public App()
    {
        Container = new IocContainer(this);
        IconProvider.Current.Register<FontAwesomeIconProvider>();

    }

    public IocContainer Container { get; protected set; }

    public void Configure(IIocContainer container)
    {
        container.Register<HomeView>();
        container.Register<MenuView>();
        container.Register<FormView>();
        container.Register<FormPage1View>();
        container.Register<FormPage2View>();
        container.Register<ModalWindow>();
    }

    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<IIocConfiguration>(this);
        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<Router>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddTransient<ViewModelBaseFactory<FormViewModel>>();
        services.AddTransient<FormPage1ViewModel>();
        services.AddTransient<FormPage2ViewModel>();
        services.AddTransient<ModalViewModel>();
    }

    public void Configure(IServiceProvider provider)
    {
        SimpleProvider.ConfigureServices(provider);
    }

    public object? GetView(string key, object[] @params)
    {
        return Container!.GetView(key, @params);
    }

    public IContextFor? GetContextFor(string key, params object[] @params)
    {
        return Container!.GetView(key, @params) as IContextFor;
    }

    /// <summary>
    /// Obtain the view by the viewModel association
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="params"></param>
    /// <returns></returns>
    public IContextFor<TViewModel>? GetContextFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase
    {
        return Container!.GetView(typeof(TViewModel).Name, @params) as IContextFor<TViewModel>;
    }

    /// <summary>
    /// Obtain the view by the modalViewModel association
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="params"></param>
    /// <returns></returns>
    public IModalFor<TViewModel>? GetModalFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase
    {
        return Container!.GetView(typeof(TViewModel).Name, @params) as IModalFor<TViewModel>;
    }

    /// <summary>
    /// Obtain the parent
    /// </summary>
    /// <returns>The TopLevel visual</returns>
    public object? GetParent()
    {
        var visual = TopLevelStateManager.GetActive();
        return visual;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(new Avae.Abstractions.Router())
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
