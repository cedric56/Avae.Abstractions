using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class HomeView : ContentPage, IContext
{
    public HomeView()
    {
        InitializeComponent();
    }

    public object? Context { get => BindingContext; set => BindingContext = value; }
}