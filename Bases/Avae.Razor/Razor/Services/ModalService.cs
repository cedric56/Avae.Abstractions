using Avae.ViewModels;

namespace Avae.Razor;

internal static class ModalService
{
    public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;

    public static async Task<TResult?> ShowModalAsync<TViewModel, TResult>(
        ViewFor view, TViewModel viewModel, NavigableContext? context)
        where TViewModel : ICloseableViewModel<TResult>
    {
        var tcs = new TaskCompletionSource<TResult?>();

        var dialog = await MudDialogService.ShowAsync(view.Type, viewModel.Title, new MudBlazor.DialogParameters()
        {
            { "ViewModel", viewModel }
        });
        viewModel.CloseRequested += CloseRequestedHandler;
        void CloseRequestedHandler(object? sender, TResult? e)
        {
            viewModel.CloseRequested -= CloseRequestedHandler;
            tcs.SetResult(e);
            MudDialogService.Close(dialog);
        }
        return await tcs.Task;
    }
}
