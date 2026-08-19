using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class HomeView : ContentPage, IViewFor<HomeViewModel>
{
	public HomeView()
	{
		InitializeComponent();
	}

    public object? Context { get => BindingContext; set => BindingContext = value; }
}