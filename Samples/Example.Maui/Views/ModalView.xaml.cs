using Avae.Maui;
using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class ModalView : ContentView, IModalFor<ModalViewModel, string?>
{
    IModalService service;
    public object? Context { get => BindingContext; set => BindingContext = value; }

    public ModalView(IModalService service)
    {
        InitializeComponent();
        this.service = service;
    }

    public Task<string?> ShowModalAsync()
    {
        return service.ShowModalAsync<ModalViewModel, string?>(BindingContext, this);
    }
}