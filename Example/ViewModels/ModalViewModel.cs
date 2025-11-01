using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class ModalViewModel : CloseableViewModelBase<string>
    {
        [RelayCommand]
        public Task Validate()
        {
            return Close("Validate");
        }

        [RelayCommand]
        public Task Cancel()
        {
            return Close("Cancel");
        }

    }
}
