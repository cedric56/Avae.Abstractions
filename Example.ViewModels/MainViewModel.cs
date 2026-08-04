using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System.Collections.ObjectModel;

namespace Example.ViewModels;

public abstract partial class ExampleViewModelBase(Router router, bool initialize = true) : PagesViewModelBase(router, initialize)
{
    [RelayCommand]
    public override void GoBack()
    {
        base.GoBack();
    }

    [RelayCommand]
    public override void GoForward()
    {
        base.GoForward();
    }

    protected override void RaiseCanExecuteChanged()
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }
}

public abstract partial class TestViewModelBase<TResult>(Router router) : FormViewModelBase<TResult>(router)
{
    [RelayCommand]
    public override void GoBack()
    {
        base.GoBack();
    }

    [RelayCommand]
    public override void GoForward()
    {
        base.GoForward();
    }

    protected override void RaiseCanExecuteChanged()
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }
}


[ObservableObject]
public partial class MainViewModel(Router router) : ExampleViewModelBase(router)
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

    protected override ObservableCollection<PageViewModelBase> GetPages()
    {
        return
        [
                new PageViewModelBase<HomeViewModel>("Home", "fa-solid fa-house"),
                new PageViewModelBase<MenuViewModel>("Menu", "fa-solid fa-gear")
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
