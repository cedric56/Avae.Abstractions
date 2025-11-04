using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public partial class ModalViewModel : CloseableViewModelBase<string>, IDataErrorInfo
    {
        private string? _text;

        [Required(ErrorMessage = "You have to enter a value.")]
        public string? Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); } 
        }

        public string Error
        {
            get
            {
                return InputValidation<ModalViewModel>.Error(this);
            }
        }

        public string this[string columnName]
        {

            get
            {
                return InputValidation<ModalViewModel>.Validate(this, columnName);
            }
        }

        [RelayCommand]
        [Closeable(0)]
        public async Task Validate()
        {
            if (await CanClose())
                await Close(Text!);
            else
                await DialogWrapper.ShowOkAsync(Error, "Error");
        }

        [RelayCommand]
        [Closeable(1)]
        public Task Cancel()
        {
            return Close("Cancel");
        }

        protected override Task<bool> CanClose()
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(Error));
        }
    }
}
