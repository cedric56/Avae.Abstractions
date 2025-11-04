using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class HomeViewModel : ObservableObject, IViewModelBase
    {
        public string Title => "Welcome to home";

        [RelayCommand]
        public async Task ShowModal()
        {
            var result = await this.ShowDialogAsync<ModalViewModel, string>();
            await DialogWrapper.ShowOkAsync(result, "Result");
        }
    }
}
