using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using Example.ViewModels.Defaults;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

[ObservableObject]
public partial class MainViewModel(Router router) : RoutesViewModelImplementation(router)
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

    protected override ObservableCollection<ViewModelDescriptor> GetViewModels()
    {
        return
        [
                new ViewModelDescriptor<HomeViewModel>("Home", "fa-solid fa-house"),
                new ViewModelDescriptor<MenuViewModel>("Menu", "fa-solid fa-gear")
                {
                    Launched = (viewModel) =>
                    {
                        viewModel.Persons = new(Repository.Instance.Persons);
                        return Task.CompletedTask;
                    }
                }
        ];
    }
}
