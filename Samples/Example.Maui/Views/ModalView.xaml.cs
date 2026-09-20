using Avae.Maui;
using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class ModalView : ContentView, IModalFor<ModalViewModel, string?>
{
    public object? Context { get => BindingContext; set => BindingContext = value; }

    public ModalView()
    {
        InitializeComponent();
    }

    public Task<string?> ShowModalAsync()
    {
        return ModalService.ShowModalAsync<ModalViewModel, string?>(BindingContext, this);
    }
}