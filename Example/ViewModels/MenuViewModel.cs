using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Dal;
using Example.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    [ObservableObject]
    internal partial class MenuViewModel(Router router) : PagesViewModelBase(router, false), IViewModelBase
    {
        public string Title { get; set; } = "Persons";

        public ObservableCollection<Person> Persons { get; set; } = new(DBBase.Instance.GetAll<Person>());

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
                return new ObservableCollection<PageViewModelBase>
                {
                    new PageViewModelBase<FormViewModel>("Form", "fa-solid fa-gear")
                };
            }
        }

        [RelayCommand]
        public void Add()
        {
            OpenForm(new Person(), Persons.Add);
        }

        [RelayCommand(CanExecute = nameof(CanExecute))]
        public void Update()
        {
            OpenForm(SelectedPerson!, person =>
            {
                Persons[Persons.IndexOf(SelectedPerson!)] = person;
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecute))]
        public async Task Remove()
        {
            var result = await SelectedPerson!.RemoveAsync();
            if (!result.Success)
                await DialogWrapper.ShowOkAsync(result.Exception, "Error");
            else
                Persons.Remove(SelectedPerson);
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
