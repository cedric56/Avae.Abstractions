using Avae.Abstractions;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class FormView : ContentView, IViewFor<FormViewModel>
{
	public FormView()
	{
		InitializeComponent();
	}

    public object? Context { get => BindingContext; set => BindingContext = value; }
}