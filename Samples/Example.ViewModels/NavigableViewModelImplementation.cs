using Avae.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Example.ViewModels;

public abstract partial class NavigableViewModelImplementation(Router router, bool initialize = true) : 
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
}

public abstract partial class NavigableViewModelImplementation<TResult>(Router router, bool initialize = true) :
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
}
