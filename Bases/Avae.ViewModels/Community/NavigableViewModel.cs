using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avae.ViewModels;

[INotifyPropertyChanged]
public abstract partial class NavigableViewModel(Router router, bool initialize = true) :
NavigableViewModelBase(router, initialize)
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

    protected override void RaiseCanExecutesChanged()
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }

    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}

[INotifyPropertyChanged]
public abstract partial class NavigableViewModel<TResult>(Router router, bool initialize = true) :
    NavigableViewModelBase<TResult>(router, initialize)
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

    protected override void RaiseCanExecutesChanged()
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }

    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
