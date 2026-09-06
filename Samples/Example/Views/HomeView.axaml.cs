using Avae.Core;
using Avae.Services;
using Avae.ViewModels;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Example.ViewModels;

namespace Example;

public partial class HomeView : UserControl, IViewFor<HomeViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public HomeView()
    {
        InitializeComponent();

        Loaded += (sender, e) =>
        {
            if (DataContext is HomeViewModel vm)
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);
                WeakReferenceMessenger.Default.Register<string, HomeViewModel>(
                    this, vm, async (o, s) =>
                    {
                        var dialogService = ServiceLocator.GetRequiredService<IDialogService>();
                        await dialogService.ShowOkAsync(s, "Message from HomeViewModel");
                    });
            }
        };
    }
}