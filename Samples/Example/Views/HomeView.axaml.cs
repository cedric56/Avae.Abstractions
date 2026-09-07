using Avae.Core;
using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Example.ViewModels;

namespace Example;

public partial class HomeView : View, IViewFor<HomeViewModel>
{
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