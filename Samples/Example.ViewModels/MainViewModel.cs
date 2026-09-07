using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Example.ViewModels;

[INotifyPropertyChanged]
public partial class MainViewModel(Router router) :
    NavigableViewModel(router)
{
    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

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
