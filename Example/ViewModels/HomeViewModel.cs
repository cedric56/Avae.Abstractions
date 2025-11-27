using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class HomeViewModel : ObservableObject, IViewModelBase
    {
        public static string Title => "Welcome to home";

        [RelayCommand]
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

        public const string TaskDialogKey = "TaskDialog";

        [RelayCommand]
        public async Task ShowTaskDialog()
        {
            var configuration = SimpleProvider.GetService<IIocConfiguration>();
            var service = SimpleProvider.GetService<ITaskDialogService>();
            await service.ShowAsync(new TaskDialogParams()
            {
                Header = "Header",
                Footer = configuration.GetView(TaskDialogKey, "Footer"),
                IconSource = configuration.GetView(TaskDialogKey, "IconSource"),
                Title = "Title",
                SubHeader = "SubHeader",
                Content = configuration.GetView(TaskDialogKey, "Content"),
                FooterVisibility = TaskDialogFooterVisibility.Auto
            },
            TaskDialogStandardResult.OK,
            TaskDialogStandardResult.Cancel);
        }

        [RelayCommand]
        public async Task ShowContentDialog()
        {
            var configuration = SimpleProvider.GetService<IIocConfiguration>();
            var service = SimpleProvider.GetService<IContentDialogService>();
            await service.ShowAsync(new ContentDialogParams()
            {
                Title = "Title",
                CloseButtonText = "Close",
                Content = configuration.GetView(TaskDialogKey, "Content"),
            });
        }
    }
}
