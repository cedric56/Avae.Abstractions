using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;

namespace Example.ViewModels.Defaults
{
    public abstract partial class ExampleViewModelBase(Router router, bool initialize = true) : 
        PagesViewModelBase(router, initialize)
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
}
