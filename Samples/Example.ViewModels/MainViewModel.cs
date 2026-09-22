using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly Dictionary<NavigableView, IViewFor> dico = [];

    [ObservableProperty]
    private IViewFor? currentView;

    [ObservableProperty]
    private NavigableView? selectedNavigable;

    public ObservableCollection<NavigableView> Navigables { get; } =
    [
        new NavigableView<HomeViewModel>("Home", "fa-solid fa-house"),
        new NavigableView<MenuViewModel>("Menu", "fa-solid fa-gear"),
        new NavigableView<EssentialsViewModel>("Essentials", "fa-solid fa-gear"),
        new NavigableView<RegionsViewModel>("Regions", "fa-solid fa-gear")
    ];

    async partial void OnSelectedNavigableChanged(NavigableView? value)
    {
        if (value is null) return;
        if (dico.TryGetValue(value, out var view))
        {
            CurrentView = view;
        }
        else
        {
            CurrentView = await router.GoToType(value.ViewModelType);
            if (CurrentView != null)
                dico.Add(value, CurrentView);
        }
    }

    public void Dispose()
    {
        dico.Clear();
    }

    Router router;

    public MainViewModel(Router router)
    {
        this.router = router;

        OnSelectedNavigableChanged(Navigables[0]);
    }
}
