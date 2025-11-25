using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public static class CollectionExtensions
    {
        public static void Update<X, Y>(this IList<Y> items, IList<X> selected, Func<X, Y, bool> predicate, Func<X, Y> add)
        {
            foreach (var x in selected)
                if (!items.Any(y => predicate(x, y)))
                    items.Add(add(x));

            var deleted = new List<Y>();
            foreach (var item in items)
                if (!selected.Any(x => predicate(x, item)))
                    deleted.Add(item);

            foreach (var item in deleted)
                items.Remove(item);
        }
    }

    [ObservableObject]
    [GoTo]
    internal partial class FormViewModel(Router router, Person person) : FormViewModelBase<Person>(router),         
        IDataErrorInfo
    {
        public const string KEY = "Page";

        [ObservableProperty]
        private bool _isBusy = false;

        public Person Person { get; } = person;

        public List<Person> Persons
        {
            get
            {
                return Repository.Instance.Persons.Where(p => p.Id != Person.Id).ToList();
            }
        }

        private List<Person> _selectedItems= [];
        public List<Person> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                SetProperty(ref  _selectedItems, value);
            }
        }

        [RelayCommand]
        public async Task Validate()
        {
            if (!string.IsNullOrWhiteSpace(Error))
                await DialogWrapper.ShowOkAsync(Error, "Error");
            else
            {
                Person.Contacts.Update(
                SelectedItems,
                (person, contact) => person.Id == contact.IdPerson,
                (person) => new Contact()
                {
                    IdPerson = person.Id,
                    Person = person,
                    PersonContact = Person
                });

                IsBusy = true;
                await Task.Delay(2000);
                var result = await DBBase.Instance.DbTransSave(Person);
                IsBusy = false;
                if (!string.IsNullOrWhiteSpace(result.Exception))
                    await DialogWrapper.ShowOkAsync(result.Exception, "Error");
                
                await Close(result.Success ? Person : null);
            }
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
                         FactoryParameters = [KEY.ForFactory()],
                         Launched = async (viewModel) =>
                            {
                                await Person.LoadContactsAsync();
                                SelectedItems = new(Person?.Contacts.Where(c=> c.Person is not null).Select(c => c.Person!) ?? []);
                            }
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

        protected override IContextFor GoTo(PageViewModelBase value, out IViewModelBase viewModel)
        {
            //Possibility to set parameters on call
            if (value?.ViewModelType == typeof(FormPage3ViewModel))
                value.ViewParameters = [Person.ForView()];

            return base.GoTo(value, out viewModel);
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
