using Avae.Avalonia;
using Avae.Services;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Example.ViewModels;
using ReactiveUI.Avalonia;

namespace Example;

public partial class HomeView : 
    ViewFor<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }

    public HomeView(IDialogService dialogService)
        : this()
    {
        Loaded += OnLoaded;

        void OnLoaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if (DataContext is HomeViewModel vm)
            {
                WeakReferenceMessenger.Default.Register<string, HomeViewModel>(
                    this, vm, async (o, s) =>
                    {
                        await dialogService.ShowOkAsync(s, "Message from HomeViewModel");
                    });
            }
        }
    }
}