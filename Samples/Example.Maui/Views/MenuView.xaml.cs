using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class MenuView : ContentPage, IViewFor<MenuViewModel>
{
	public MenuView()
	{
		InitializeComponent();
	}

    public object? Context { get => BindingContext; set => BindingContext = value; }
}