using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Example.Models;
using System;
using System.Collections.ObjectModel;

namespace Example.ViewModels
{
    internal partial class MenuViewModel(Router router) : PagesViewModelBase(router, false), IViewModelBase
    {
        public ObservableCollection<Person> Persons { get; set; } = new();

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

        [RelayCommand(CanExecute=nameof(CanExecute))]
        public void Remove()
        {
            Persons.Remove(SelectedPerson!);
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
    }
}
