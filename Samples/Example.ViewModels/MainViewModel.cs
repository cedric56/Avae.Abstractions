using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private IViewFor? currentView;

    [ObservableProperty]
    private NavigableView? selectedNavigable;

    public ObservableCollection<NavigableView> Navigables { get; } =
    [
        new NavigableView<HomeViewModel>("Home", "fa-solid fa-house"),
        new NavigableView<MenuViewModel>("Menu", "fa-solid fa-gear"),
        new NavigableView<EssentialsViewModel>("Essentials", "fa-solid fa-gear")
    ];

    partial void OnSelectedNavigableChanged(NavigableView? value)
    {
        if (value is null) return;
        CurrentView = router.GoTo(value.ViewModelType, out var wm);
    }

    Router router;

    public MainViewModel(Router router)
    {
        this.router = router;
        CurrentView = router.GoTo(Navigables[0].ViewModelType);
    }
}
