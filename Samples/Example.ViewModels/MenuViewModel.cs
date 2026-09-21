using Avae.DAL;
using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;
using Person = Example.Models.Person;

namespace Example.ViewModels;

public partial class MenuViewModel : NavigableViewModel
{
    IServiceProvider provider;
    IDialogService dialogService;
    IDBFactory factory;

    //IDispatcher dispatcher;
    public MenuViewModel(IServiceProvider provider, 
        IDBFactory factory,
        //IDispatcher dispatcher,
        IDialogService dialogService, Router router)
        : base(router, false)
    {
        //this.dispatcher = dispatcher;
        this.provider = provider;
        this.factory = factory;
        this.dialogService = dialogService;

        Repository.Instance.PersonsChanged += OnPersonsChanged;
    }

    private void OnPersonsChanged(object? sender, EventArgs e)
    {
        Persons = new(Repository.Instance.Persons);
    }

    public string Title { get; set; } = "Persons";

    [ObservableProperty]
    private ObservableCollection<Person> _persons = new(Repository.Instance.Persons);

    [ObservableProperty]
    private Person? _selectedPerson;

    partial void OnSelectedPersonChanged(Person? value)
    {
        UpdateCommand.NotifyCanExecuteChanged();
        RemoveCommand.NotifyCanExecuteChanged();
    }

    protected override ObservableCollection<NavigableView> GetNavigables()
    {
        return
        [
            new NavigableView<FormViewModel>("Form", "fa-solid fa-gear")
        ];
    }

    [RelayCommand]
    public Task Add()
    {
        return OpenForm(new Person(), person =>
        {
            Persons.Add(person);
            SelectedPerson = person;
        });
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    public Task Update()
    {
        return OpenForm(SelectedPerson!, person =>
        {
            Persons[Persons.IndexOf(SelectedPerson!)] = person;
            SelectedPerson = person;
        });
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    public async Task Remove()
    {
        await SelectedPerson!.LoadContactsAsync();
        var result = await SelectedPerson.Remove(DBBase.Instance, factory);//.Remove(SelectedPerson);
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

    public async Task OpenForm(Person person, Action<Person> action)
    {
        var viewModel = new FormViewModel(//dispatcher, 
            dialogService, provider.GetRequiredService<Router>(), person);

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

        CurrentView = await _router.GoTo(viewModel);
    }

    public override void Dispose()
    {
        base.Dispose();
        Repository.Instance.PersonsChanged -= OnPersonsChanged;
    }
}
