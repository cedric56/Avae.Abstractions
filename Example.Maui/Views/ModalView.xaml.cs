using Avae.Maui;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class ModalView : ContentView, IDialogView<ModalViewModel, string?>
{
    public object? Context { get => BindingContext; set => BindingContext = value; }

    public ModalView()
    {
		InitializeComponent();
	}

    public string Title => "Modal";
}