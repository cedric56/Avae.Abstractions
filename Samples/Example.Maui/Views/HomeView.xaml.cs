using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class HomeView : ContentView, IViewFor
{
    public HomeView()
    {
        InitializeComponent();
    }

    public object? Context { get => BindingContext; set => BindingContext = value; }
}