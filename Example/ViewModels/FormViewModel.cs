using Avae.Abstractions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal class FormViewModel(Router router) : FormViewModelBase(router)
    {
        public override string Title => "Form";

        public override ObservableCollection<PageViewModelBase> Pages
        {
            get
            {
                return new ObservableCollection<PageViewModelBase>
                {
                    new PageViewModelBase<FormPage1ViewModel>("Page One", "fa-solid fa-gear"),
                    new PageViewModelBase<FormPage2ViewModel>("Page Two", "fa-solid fa-gear"),
                };
            }
        }

        protected override Task<bool> CanClose()
        {
            return DialogWrapper.ShowDialog("Are you sure you want to close ?");
        }
    }
}
