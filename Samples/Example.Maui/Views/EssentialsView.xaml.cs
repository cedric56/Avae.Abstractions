using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class EssentialsView : ContentView, IViewFor<EssentialsViewModel>
{
	public EssentialsView()
	{
		InitializeComponent();
	}
    public object? Context { get => BindingContext; set => BindingContext = value; }
}