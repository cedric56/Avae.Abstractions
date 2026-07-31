using Avae.Abstractions;
using Avae.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Example.ViewModels
{
    public partial class HomeViewModel(
        IDialogService dialogService,
        IContentDialogService contentDialogService,
        ITaskDialogService taskDialogService,
        IIocConfiguration iocConfiguration) : ObservableObject, IViewModelBase
    {
        public static string Title => "Welcome to home";

        [RelayCommand]
        public async Task ShowModal()
        {
            string? result = string.Empty;
            try
            {
                result = await dialogService.ShowModalAsync<ModalViewModel, string?>();
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                await dialogService.ShowOkAsync(result ?? string.Empty, "Result");
            }
        }

        public const string TaskDialogKey = "TaskDialog";

        [RelayCommand]
        public async Task ShowTaskDialog()
        {
            await taskDialogService.ShowAsync(new TaskDialogParams()
            {
                Header = "Header",
                Footer = iocConfiguration.GetView(TaskDialogKey, "Footer"),
                IconSource = iocConfiguration.GetView(TaskDialogKey, "IconSource"),
                Title = "Title",
                SubHeader = "SubHeader",
                Content = iocConfiguration.GetView(TaskDialogKey, "Content"),
                FooterVisibility = TaskDialogFooterVisibility.Auto
            },
            TaskDialogStandardResult.OK,
            TaskDialogStandardResult.Cancel);
        }

        [RelayCommand]
        public async Task ShowContentDialog()
        {
            await contentDialogService.ShowAsync(new ContentDialogParams()
            {
                Title = "Title",
                CloseButtonText = "Close",
                Content = iocConfiguration.GetView(TaskDialogKey, "Content"),
            });
        }
    }
}
