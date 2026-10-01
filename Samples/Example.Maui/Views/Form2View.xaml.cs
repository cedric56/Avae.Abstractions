using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class Form2View : ContentView, IViewFor
{
    public object? Context { get => BindingContext; set => BindingContext = value; }

    public Form2View()
	{
		InitializeComponent();
	}
}