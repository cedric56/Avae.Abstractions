using Avae.ViewModels;

namespace Example.Maui.Views;

public partial class Form1View : ContentView, IViewFor
{
	public Form1View()
	{
		InitializeComponent();
	}
    public object? Context { get => BindingContext; set => BindingContext = value; }

}