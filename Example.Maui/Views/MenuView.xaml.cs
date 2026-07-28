using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class MenuView : ContentPage, IContextFor<MenuViewModel>
{
	public MenuView()
	{
		InitializeComponent();
	}

    public object? Context { get => BindingContext; set => BindingContext = value; }
}