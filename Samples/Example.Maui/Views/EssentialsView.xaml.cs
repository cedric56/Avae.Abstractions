using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class EssentialsView : ContentPage, IViewFor
{
    public EssentialsView()
    {
        InitializeComponent();
    }
    public object? Context { get => BindingContext; set => BindingContext = value; }
}