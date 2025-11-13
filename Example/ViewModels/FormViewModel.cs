using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    [ObservableObject]
    internal partial class FormViewModel(Router router, Person person) : FormViewModelBase<Person>(router), IDataErrorInfo
    {
        public Person Person { get; } = person;

        [RelayCommand]
        public async Task Validate()
        {
            if (!string.IsNullOrWhiteSpace(Error))
                await DialogWrapper.ShowOkAsync(Error, "Error");
            else
                await Close(Person);
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
                        //Possibility to set parameters on ctor
                         //ViewParameters = [Person.ForView()]
                    }
                };
            }
        }

        protected override IContextFor GoTo(PageViewModelBase value)
        {
            //Possibility to set parameters on call
            if(value?.ViewModelType == typeof(FormPage3ViewModel))
                value.ViewParameters = [Person.ForView()];

            return base.GoTo(value);
        }

        public string Error => Person.Error;

        public string this[string columnName] => Person[columnName];

        protected override Task<bool> CanClose()
        {
            return DialogWrapper.ShowYesNoAsync("Are you sure you want to close ?", "Question"); 
        }

        protected override void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
