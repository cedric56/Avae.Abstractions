using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable, IMvvmManager
{
    private readonly Dictionary<NavigableView, IViewFor> dico = [];

    [ObservableProperty]
    private IViewFor? currentView;

    [ObservableProperty]
    private NavigableView? selectedNavigable;

    public ObservableCollection<NavigableView> Navigables { get; } =
    [
        new NavigableView<HomeViewModel>("Home", "fa-solid fa-house")
        {
            Href = "/"
        },
        new NavigableView<MenuViewModel>("Menu", "fa-solid fa-gear")
        {
            Href = "menu"
        },
        new NavigableView<EssentialsViewModel>("Essentials", "fa-solid fa-gear")
        {
            Href = "essentials"
        },
        new NavigableView<RegionsViewModel>("Regions", "fa-solid fa-gear")
        {
            Href = "regions"
        }
    ];

    async partial void OnSelectedNavigableChanged(NavigableView? value)
    {
        await OnNavigableChanged(value);
    }

    public void Dispose()
    {
        dico.Clear();
    }

    async public Task OnNavigableChanged(NavigableView? value)
    {
        if (value is null) return;
        if (dico.TryGetValue(value, out var view))
        {
            CurrentView = view;
        }
        else
        {
            var context = await router.GoToType(value.ViewModelType, context: NavigableContext.Create().WithAdditionalParameters(("Test", "This is a value pass as parameter")));
            if (context.view != null)
            {
                CurrentView = context.view;
                dico.Add(value, context.view);
            }
        }
    }

    Router router;

    public MainViewModel(Router router)
    {
        this.router = router;

        OnSelectedNavigableChanged(Navigables[0]);
    }
}
