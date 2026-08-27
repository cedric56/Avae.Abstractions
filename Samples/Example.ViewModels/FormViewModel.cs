using Avae.ViewModels;
using Avae.DAL;
using Avae.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using Example.ViewModels.Defaults;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Person = Example.Models.Person;
namespace Example.ViewModels;

[INotifyPropertyChanged]
public partial class FormViewModel(IDialogService dialogService, Router router, Person person) : FormViewModelImplementation<Person>(router), IDataErrorInfo
{
    public const string KEY = "Page";

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public Person Person { get; private set; } = person;

    public List<Person> Persons
    {
        get
        {
            return [.. Repository.Instance.Persons.Where(p => p.Id != Person.Id)];
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
            await dialogService.ShowOkAsync(Error, "Error");
        else
        {
            IsBusy = true;
            Person.Contacts.Update(
            SelectedItems,
            (person, contact) => person.Id == contact.IdPerson,
            (person) => new Contact()
            {
                IdPerson = person.Id,
                Person = person,
                PersonContact = Person
            });
            var result = await DBBase.Instance.Save(Person);
            IsBusy = false;
            if (!string.IsNullOrWhiteSpace(result.Exception))
                await dialogService.ShowOkAsync(result.Exception, "Error");
            
            await Close(result.Successful ? Person : null);
        }
    }

    public override string Title => "Form";

    protected override ObservableCollection<ViewDescriptor> GetViewModels()
    {
        return new ObservableCollection<ViewDescriptor>
            {
                new ViewDescriptor<FormViewModel>(this, "Page One", "fa-solid fa-gear")
                {
                     NavigationContext = new NavigationContext
                     {
                         FactoryParameters = [KEY]
                     },
                     Launched = async (viewModel) =>
                     {
                        await Person.LoadContactsAsync();
                        SelectedItems = [.. Person.Contacts.Select(c => c.Person)];
                     }
                },
                new ViewDescriptor<FormPage2ViewModel>("Page Two", "fa-solid fa-gear"),
                new ViewDescriptor<FormPage3ViewModel>("Page Three", "fa-solid fa-gear")
                {
                    //Possibility to set parameters on ctor
                    //ViewParameters = [Person]
                }
            };
    }

    protected override IViewFor GoTo(ViewDescriptor value, out IViewModelBase viewModel)
    {
        //Possibility to set parameters on call
        if (value.ViewModelType == typeof(FormPage3ViewModel))
            value.NavigationContext.ViewParameters = [Person];

        return base.GoTo(value, out viewModel);
    }

    public string Error => Person.Error;

    public string this[string columnName] => Person[columnName];

    public override Task<bool> CanClose()
    {
        return dialogService.ShowYesNoAsync("Are you sure you want to close ?", "Question"); 
    }

    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
