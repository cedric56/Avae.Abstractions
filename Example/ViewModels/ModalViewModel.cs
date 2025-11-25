using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Example.ViewModels
{
    public partial class ModalViewModel : ReactiveObject, 
        ICloseableViewModel<string>,
        IDataErrorInfo
    {
        static ModalViewModel()
        {
            InputValidation<ModalViewModel>.Init();
        }

        public ModalViewModel()
        {
            this.WhenAnyValue(x => x.Text)
                .Skip(1)// ⛔ ignore the initial value
                .Throttle(TimeSpan.FromSeconds(1))
                .Where(string.IsNullOrWhiteSpace)
                .ObserveOn(SynchronizationContext.Current!)
                .InvokeCommand(ValidateCommand);
        }

        private string? _text;

        public event EventHandler<string?>? CloseRequested;

        [Required(ErrorMessage = "You have to enter a value.")]
        public string? Text
        {
            get { return _text; }
            set { this.RaiseAndSetIfChanged(ref _text, value); } 
        }

        public string Error
        {
            get
            {
                return InputValidation<ModalViewModel>.Error(this);
            }
        }

        public ICommand? CloseCommand { get; }

        public CommandIndex[] Commands => 
            [
                new() { Command = ValidateCommand, Index = 0},
                new() { Command = CancelCommand, Index = 1}
            ];

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
                await Close(Text!);
            else
                await DialogWrapper.ShowOkAsync(Error, "Error");
        }

        [RelayCommand]
        public Task Cancel()
        {
            return Close("Cancel");
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
    }
}
