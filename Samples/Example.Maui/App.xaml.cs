using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui;

public partial class App : Application
{
    IServiceProvider provider;

    public App(IServiceProvider provider)
    {
        InitializeComponent();
        this.provider = provider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(new MainPage() { BindingContext = provider.GetViewModel<MainViewModel>() });
        return new Window(new AppShell(provider));
    }
}