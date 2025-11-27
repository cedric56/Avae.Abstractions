using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    [ObservableObject]
    internal partial class MenuViewModel(Router router) : PagesViewModelBase(router, false)
    {
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

        public override ObservableCollection<PageViewModelBase> Pages
        {
            get
            {
                return
                [
                    new PageViewModelBase<FormViewModel>("Form", "fa-solid fa-gear")
                ];
            }
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
                await DialogWrapper.ShowOkAsync(result.Exception!, "Error");
            else
            {
                Persons.Remove(SelectedPerson);
            }
        }

        private bool CanExecute()
        {
            return SelectedPerson != null;
        }

        public void OpenForm(Person person, Action<Person> action)
        {
            var viewModel = new FormViewModel(SimpleProvider.GetService<Router>(), person);

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
    }
}
