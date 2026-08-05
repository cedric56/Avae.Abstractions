using Avae.Abstractions;
using Avae.Services;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Example.ViewModels
{
    public partial class ModalViewModel : ReactiveObject, 
        ICloseableViewModel<string?>,
        IViewModelErrorInfo
    {
        static ModalViewModel()
        {
            InputValidation<ModalViewModel>.Init();
        }

        IDialogService dialogService;

        public ModalViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            //this.WhenAnyValue(x => x.Message)
            //    //.Skip(1)
            //    //.Throttle(TimeSpan.FromSeconds(1))
            //    //.Where(string.IsNullOrWhiteSpace)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(text => this.RaisePropertyChanged("Item"));
        }

        [ReactiveUI.SourceGenerators.Reactive]
        [Required(ErrorMessage = "You have to enter a value.")]
        private string? _message;        

        public event EventHandler<string?>? CloseRequested;

        public string Error
        {
            get
            {
                return InputValidation<ModalViewModel>.Error(this);
            }
        }

        public ICommand? CloseCommand { get; }

        public ObservableCollection<NamedCommand> Commands =>
            [
                new() { Command = ValidateCommand, Name = "Valider"},
                new() { Command = CancelCommand, Name="Annuler"}
            ];

        public string? Title => "Modal";

        public string this[string columnName]
        {

            get
            {
                return InputValidation<ModalViewModel>.Validate(this, columnName);
            }
        }

        [RelayCommand()]
        public async Task Validate()
        {
            if (await CanClose())
                await Close(Message!);
            else
                await dialogService.ShowOkAsync(Error, "Error");
        }

        [RelayCommand]
        public Task Cancel()
        {
            return Close(null);
        }

        protected Task<bool> CanClose()
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(Error));
        }

        public Task Close(string? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }

        public void RaiseErrorChanged()
        {
            this.RaisePropertyChanged("Item");
        }
    }
}
