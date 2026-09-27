using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable, IMvvmManager
{
    private readonly Dictionary<NavigableView, (IViewFor view, object viewmodel)> dico = [];

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
        foreach (var pair in dico)
        {
            (pair.Key as IDisposable)?.Dispose();
            (pair.Value.view as IDisposable)?.Dispose();
            (pair.Value.viewmodel as IDisposable)?.Dispose();
        }

        dico.Clear();
    }

    async public Task OnNavigableChanged(NavigableView? value)
    {
        if (value is null) return;
        if (dico.TryGetValue(value, out var tuple))
        {
            CurrentView = tuple.view;
        }
        else
        {
            var context = await router.GoToType(value.ViewModelType, context: NavigableContext.Create().WithAdditionalParameters(("Test", "This is a value pass as parameter")));
            if (context.view != null)
            {
                CurrentView = context.view;
                dico.TryAdd(value, (context.view, context.viewmodel));
            }
        }
    }

    Router router;

    public MainViewModel(Router router)
    {
        this.router = router;

        //SelectedNavigable = Navigables[0];
        _ = OnNavigableChanged(Navigables[0]);
    }
}
