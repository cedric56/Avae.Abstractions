using Avae.Services;
using Avae.ViewModels;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Example.ViewModels;

namespace Example;

public partial class HomeView : View, IViewFor<HomeViewModel>
{
    public HomeView(IDialogService dialogService)
    {
        InitializeComponent();

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