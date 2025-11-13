using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

[ObservableObject]
public partial class MainViewModel(Router router) : PagesViewModelBase(router)
{
    [ObservableProperty]
    private bool _isMenuPaneOpen;

    [RelayCommand]
    private void TriggerMenuPane()
    {
        IsMenuPaneOpen = !IsMenuPaneOpen;
    }

    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

    public override ObservableCollection<PageViewModelBase> Pages
    {
        get
        {
            return new ObservableCollection<PageViewModelBase>
            {
                new PageViewModelBase<HomeViewModel>("Home", "fa-solid fa-house"),
                new PageViewModelBase<MenuViewModel>("Menu", "fa-solid fa-gear"),
            };
        }
    }
}
