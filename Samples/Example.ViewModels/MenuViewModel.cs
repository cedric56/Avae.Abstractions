using Avae.ViewModels;
using Avae.DAL;
using Avae.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.ViewModels.Defaults;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using Person = Example.Models.Person;
using Example.Models;

namespace Example.ViewModels;

[INotifyPropertyChanged]
public partial class MenuViewModel : RoutesViewModelImplementation, IDisposable
{
    IServiceProvider provider;
    IDialogService dialogService;

    public MenuViewModel(IServiceProvider provider, IDialogService dialogService, Router router)
        :base(router,false)
    {
        this.provider = provider;
        this.dialogService = dialogService;

        Repository.Instance.PersonsChanged += OnPersonsChanged;
    }

    private void OnPersonsChanged(object? sender, EventArgs e)
    {
        Persons = new(Repository.Instance.Persons);
    }

    public string Title { get; set; } = "Persons";

    [ObservableProperty]
    public partial ObservableCollection<Person> Persons { get; set; } = new();

    [ObservableProperty]
    public partial Person? SelectedPerson { get; set; }

    partial void OnSelectedPersonChanged(Person? value)
    {
        UpdateCommand.NotifyCanExecuteChanged();
        RemoveCommand.NotifyCanExecuteChanged();
    }

    protected override ObservableCollection<ViewDescriptor> GetViewModels()
    {
        return
        [
            new ViewDescriptor<FormViewModel>("Form", "fa-solid fa-gear")
        ];
    }

    [RelayCommand]
    public void Add()
    {
        OpenForm(new Person(), person =>
        {
            Persons.Add(person);
            SelectedPerson = person;
        });
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    public void Update()
    {
        OpenForm(SelectedPerson!, person =>
        {
            Persons[Persons.IndexOf(SelectedPerson!)] = person;
            SelectedPerson = person;
        });
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    public async Task Remove()
    {
        await SelectedPerson!.LoadContactsAsync();
        var result = await DBBase.Instance.Remove(SelectedPerson);
        if (!result.Successful)
        {
            await dialogService.ShowOkAsync(result.Exception!, "Error");
        }
        else
        {
            Persons.Remove(SelectedPerson);
        }
    }

    public bool CanExecute()
    {
        return SelectedPerson != null;
    }

    public void OpenForm(Person person, Action<Person> action)
    {
        var viewModel = new FormViewModel(dialogService, provider.GetRequiredService<Router>(), person);

        EventHandler<Person?>? closeRequested = null!;
        viewModel.CloseRequested += closeRequested = (sender, e) =>
        {
            viewModel.CloseRequested -= closeRequested;
            if (e is not null)
            {
                action(e);                    
            }

            CurrentView = null!;
        };

        CurrentView = _router.GoTo(viewModel);
    }

    protected override void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

    public void Dispose()
    {
        Repository.Instance.PersonsChanged -= OnPersonsChanged;
    }
}
