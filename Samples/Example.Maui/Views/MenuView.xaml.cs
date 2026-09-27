using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class MenuView : ContentPage, IViewFor
{
    public MenuView()
    {
        InitializeComponent();
    }

    public object? Context { get => BindingContext; set => BindingContext = value; }
}