using Avae.Services;
using Avae.ViewModels;
using UXDivers.Popups.Services;

namespace Avae.Maui.Services;

internal class DialogService(Helper helper, IServiceProvider provider, IIocConfiguration configuration) : IDialogService
{        
    /// <summary>
    /// Displays an alert showing the specified exception's message.
    /// </summary>
    /// <param name="ex">The exception whose message should be displayed.</param>
    /// <param name="title">The dialog title. Defaults to "Error".</param>
    /// <returns>A task representing the asynchronous display operation.</returns>
    public Task ShowErrorAsync(Exception ex, string title = "Error")
    {
        return helper.Current.DisplayAlertAsync(title, ex.Message, "Ok");
    }

    /// <summary>
    /// Displays an alert with a single "Ok" button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>A task representing the asynchronous display operation.</returns>
    public Task ShowOkAsync(string message, string title = "Title")
    {
        return helper.Current.DisplayAlertAsync(title, message, "Ok");
    }

    /// <summary>
    /// Displays an alert with "Yes" and "No" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Yes"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowYesNoAsync(string message, string title = "Title")
    {
        return helper.Current.DisplayAlertAsync(title, message, "Yes", "No");
    }

    /// <summary>
    /// Displays an alert with "Ok" and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkCancelAsync(string message, string title = "Title")
    {
        return helper.Current.DisplayAlertAsync(title, message, "Ok", "Cancel");
    }

    /// <summary>
    /// Displays an alert with "Ok" and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns><see langword="true"/> if the user selected "Ok"; otherwise <see langword="false"/>.</returns>
    public Task<bool> ShowOkAbortAsync(string message, string title = "Title")
    {
        return helper.Current.DisplayAlertAsync(title, message, "Ok", "Abort");
    }

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Cancel" was selected.</returns>
    public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
    {
        return helper.DisplayThreeButtons(title, message, "Yes", "No", "Cancel", 0, 1, 2);
    }

    /// <summary>
    /// Displays an alert with "Yes", "No", and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Title".</param>
    /// <returns>0 if "Yes" was selected, 1 if "No" was selected, or 2 if "Abort" was selected.</returns>
    public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
    {
        return helper.DisplayThreeButtons(title, message, "Yes", "No", "Abort", 0, 1, 2);
    }

    /// <summary>
    /// Resolves the view model of type <typeparamref name="TViewModel"/>, wraps its associated modal
    /// view in a popup page, and pushes it, resolving the returned task when the view model requests a close.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type to show.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">Optional navigation context; an empty context is used if not supplied.</param>
    /// <returns>The result value supplied when the view model requested a close.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no modal view is registered for <typeparamref name="TViewModel"/>.</exception>
    async Task<TResult?> IDialogService.ShowModalAsync<TViewModel, TResult>(NavigableContext? context)
        where TResult : default
    {
        helper.Ensure();

        var viewModel = provider.GetViewModel<TViewModel>(context);
        var view = configuration.GetModalFor<TViewModel, TResult>(context ?? new NavigableContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
        view.Context = viewModel;
        var modal = new AvaePopupPage<TResult>(viewModel.Title, viewModel.Commands)
        {
            Content = view as Microsoft.Maui.Controls.View
        };
        viewModel.CloseRequested += CloseRequestedHandler;
        return await IPopupService.Current.PushAsync(modal);

        async void CloseRequestedHandler(object? sender, TResult? e)
        {
            viewModel.CloseRequested -= CloseRequestedHandler;
            modal.SetResult(e);
            await IPopupService.Current.PopAsync(modal);
        }
    }        
}
