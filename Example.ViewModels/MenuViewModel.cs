using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using Avae.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Example.ViewModels
{
    [ObservableObject]
    public partial class MenuViewModel : ExampleViewModelBase, IDisposable
    {
        IServiceProvider provider;
        ISqlMonitor<Person> monitor;

        IDialogService dialogService;
        public MenuViewModel(IServiceProvider provider, ISqlMonitor<Person> monitor, IDialogService dialogService, Router router)
            :base(router,false)
        {
            this.provider = provider;
            this.monitor = monitor;
            this.dialogService = dialogService;

            this.monitor.OnChanged += Monitor_OnChanged;
        }

        private void Monitor_OnChanged(object? sender, IRecord<Person> e)
        {
            Persons = new(DBBase.Instance.GetAll<Person>());
        }

        public string Title { get; set; } = "Persons";

        [ObservableProperty]
        private ObservableCollection<Person> _persons = [];

        [ObservableProperty]
        private Person? _selectedPerson = null;

        partial void OnSelectedPersonChanged(Person? value)
        {
            UpdateCommand.NotifyCanExecuteChanged();
            RemoveCommand.NotifyCanExecuteChanged();
        }

        protected override ObservableCollection<PageViewModelBase> GetPages()
        {
            return
            [
                    new PageViewModelBase<FormViewModel>("Form", "fa-solid fa-gear")
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
            var result = await DBBase.Instance.DbTransRemove(SelectedPerson);
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

            EventHandler<Person>? closeRequested = null!;
            viewModel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewModel.CloseRequested -= closeRequested;
                if (e is not null)
                {
                    action(e);                    
                }

                CurrentPage = null!;
            };

            CurrentPage = _router.GoTo(viewModel);            
        }

        protected override void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void Dispose()
        {
            this.monitor.OnChanged -= Monitor_OnChanged;
        }
    }
}
