using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Example.ViewModels;

namespace Example.Maui.Views;

public partial class HomeView : ContentView, IViewFor
{
    public HomeView(IDialogService dialogService)
    {
        InitializeComponent();

        Loaded += OnLoaded;

        void OnLoaded(object? sender, EventArgs e)
        {
            Loaded -= OnLoaded;
            if (Context is HomeViewModel vm)
            {
                WeakReferenceMessenger.Default.Register<string, HomeViewModel>(
                    this, vm, async (o, s) =>
                    {
                        await dialogService.ShowOkAsync(s, "Message from HomeViewModel");
                    });
            }
        }
    }

    public object? Context { get => BindingContext; set => BindingContext = value; }
}