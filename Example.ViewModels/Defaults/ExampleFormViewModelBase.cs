using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;

namespace Example.ViewModels.Defaults
{
    public abstract partial class ExampleFormViewModelBase<TResult>(Router router, bool initialize = true) : 
        FormViewModelBase<TResult>(router, initialize)
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
}
