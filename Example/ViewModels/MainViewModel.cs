using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public partial class MainViewModel(Router router) : PagesViewModelBase(router)
{
    [ObservableProperty]
    private bool _isMenuPaneOpen;

    [RelayCommand]
    private void TriggerMenuPane()
    {
        IsMenuPaneOpen = !IsMenuPaneOpen;
    }

    public override ObservableCollection<PageViewModelBase> Pages
    {
        get
        {
            return new ObservableCollection<PageViewModelBase>
            {
                new PageViewModelBase(typeof(HomeViewModel), "Home", "fa-solid fa-house"),
                new PageViewModelBase(typeof(MenuViewModel), "Menu", "fa-solid fa-gear"),
            };
        }
    }
}
