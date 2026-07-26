using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class HomeView : ContentView, IContextFor<HomeViewModel>
{
	public HomeView()
	{
		InitializeComponent();
	}

    public object? Context { get => BindingContext; set => BindingContext = value; }
}