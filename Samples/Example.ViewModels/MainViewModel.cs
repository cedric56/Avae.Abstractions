using Avae.ViewModels;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel(Router router) :
    NavigableViewModel(router)
{
    protected override ObservableCollection<NavigableView> GetNavigables()
    {
        return
        [
            new NavigableView<HomeViewModel>("Home", "fa-solid fa-house"),
            new NavigableView<MenuViewModel>("Menu", "fa-solid fa-gear"),
            new NavigableView<EssentialsViewModel>("Essentials", "fa-solid fa-gear")
        ];
    }
}
