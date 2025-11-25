using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class HomeViewModel : ObservableObject, IViewModelBase
    {
        public string Title => "Welcome to home";

        [RelayCommand()]
        public async Task ShowModal()
        {
            string? result = string.Empty;
            try
            {
                result = await this.ShowDialogAsync<ModalViewModel, string>();
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                await DialogWrapper.ShowOkAsync(result ?? string.Empty, "Result");
            }
        }
    }
}
