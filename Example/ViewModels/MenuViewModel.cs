using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace Example.ViewModels
{
    internal partial class MenuViewModel(Router router) : PagesViewModelBase(router, false), IViewModelBase
    {
        public override ObservableCollection<PageViewModelBase> Pages
        {
            get
            {
                return new ObservableCollection<PageViewModelBase>
                {
                    new PageViewModelBase<FormViewModel>("Form", "fa-solid fa-gear"),
                };
            }
        }

        [RelayCommand]
        public void OpenForm()
        {
            //Another example how to use router without DI
            var vm = _router.GoToForm(SimpleProvider.GetService<Router>());
            var viewModel = _router.GoTo(new FormViewModel(SimpleProvider.GetService<Router>()));
            EventHandler<bool>? closeRequested = null!;
            viewModel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewModel.CloseRequested -= closeRequested;
                CurrentPage = null!;
            };
        }
    }
}
