using Avae.ViewModels;

namespace Avae.Razor;

public static class ModalService
{
    public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;

    public static async Task<TResult?> ShowModalAsync<TViewModel, TResult>(
        this IServiceProvider provider, NavigableContext? context)
        where TViewModel : ICloseableViewModel<TResult>
    {
        var tcs = new TaskCompletionSource<TResult?>();
        var viewModel = provider.GetViewModel<TViewModel>(context);
        var contextFor = provider.GetContextFor(typeof(TViewModel).Name, context ?? new NavigableContext());
        if (contextFor is ViewFor view)
        {
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

        throw new InvalidOperationException("View must be ComponentView");
    }
}
