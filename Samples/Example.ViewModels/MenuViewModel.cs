using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System.Collections.ObjectModel;
using Person = Example.Models.Person;

namespace Example.ViewModels;

public partial class MenuViewModel : NavigableViewModel, INavigable
{
    IDialogService dialogService;
    IPersonService personService;
    IEntityCache<Person> inMemoryEntityCache;
    //IDispatcher dispatcher;
    public MenuViewModel(
        IPersonService personService,
        IEntityCache<Person> inMemoryEntityCache,
        //IDispatcher dispatcher,
        IDialogService dialogService, IRouter router)
        : base(router, false)
    {
        //this.dispatcher = dispatcher;
        this.inMemoryEntityCache = inMemoryEntityCache;
        this.personService = personService;
        this.dialogService = dialogService;
        this._persons = new(inMemoryEntityCache.Entities);

        inMemoryEntityCache.EntitiesChanged += OnPersonsChanged;
    }

    public async Task OnNavigatedTo(NavigableContext context)
    {
        await inMemoryEntityCache.LoadEntities();
        Persons = new(inMemoryEntityCache.Entities);
    }

    private void OnPersonsChanged(object? sender, EventArgs e)
    {
        Persons = new(inMemoryEntityCache.Entities);
    }

    public string Title { get; set; } = "Persons";

    [ObservableProperty]
    private ObservableCollection<Person> _persons;

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
        await personService.LoadContactsAsync(SelectedPerson!);
        var result = await personService.RemoveAsync(SelectedPerson!);
        if (!result.Successful)
        {
            await dialogService.ShowOkAsync(result.Exception!, "Error");
        }
    }

    public bool CanExecute()
    {
        return SelectedPerson != null;
    }

    public async Task OpenForm(Person person, Action<Person> action)
    {
        var context = await _router.GoTo<FormViewModel>(context: NavigableContext.Create().WithViewModelParameters(person));
        EventHandler<Person?>? closeRequested = null!;
        context.viewmodel.CloseRequested += closeRequested = (sender, e) =>
        {
            context.viewmodel.CloseRequested -= closeRequested;
            if (e is not null)
            {
                action(e);
            }

            CurrentView = null!;
        };
        CurrentView = context.view;
    }

    public override void Dispose()
    {
        base.Dispose();
        inMemoryEntityCache.EntitiesChanged -= OnPersonsChanged;
    }
}
