using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class FormViewModel(Router router, Person person) : FormViewModelBase<Person>(router)
    {
        public Person Person { get; } = person;

        [RelayCommand]
        public Task Validate()
        {
            Close(Person);
            return Task.CompletedTask;
        }

        public override string Title => "Form";

        public override ObservableCollection<PageViewModelBase> Pages
        {
            get
            {
                return new ObservableCollection<PageViewModelBase>
                {
                    new PageViewModelBase<FormViewModel>(this, "Page One", "fa-solid fa-gear")
                    {
                         FactoryParameters = ["Page".ForFactory()]
                    },
                    new PageViewModelBase<FormPage2ViewModel>("Page Two", "fa-solid fa-gear"),
                    new PageViewModelBase<FormPage3ViewModel>("Page Three", "fa-solid fa-gear")
                    {
                         ViewModelParameters = [Person.ForViewModel()]
                    }
                };
            }
        }

        protected override Task<bool> CanClose()
        {
            return DialogWrapper.ShowYesNoAsync("Are you sure you want to close ?", "Question"); 
        }
    }
}
