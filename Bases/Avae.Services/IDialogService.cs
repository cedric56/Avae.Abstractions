using Avae.ViewModels;

namespace Avae.Services;

/// <summary>
/// Displays common, platform-agnostic dialog prompts (errors, confirmations,
/// and modal ViewModel-backed dialogs) without the caller needing to know
/// whether the app is running on Avalonia, MAUI, or Blazor.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an error dialog presenting the given exception's message
    /// (and, depending on the platform adapter, optionally its stack trace)
    /// with a single acknowledgement button.
    /// </summary>
    /// <param name="ex">The exception to display.</param>
    /// <param name="title">The dialog title. Defaults to "Error".</param>
    Task ShowErrorAsync(Exception ex, string title = "Error");

    /// <summary>
    /// Shows an informational dialog with a single "OK" button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    Task ShowOkAsync(string message, string title = "Title");

    /// <summary>
    /// Shows a confirmation dialog with "Yes" and "No" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if "Yes" was chosen; otherwise <see langword="false"/>.</returns>
    Task<bool> ShowYesNoAsync(string message, string title = "Title");

    /// <summary>
    /// Shows a confirmation dialog with "OK" and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if "OK" was chosen; otherwise <see langword="false"/>.</returns>
    Task<bool> ShowOkCancelAsync(string message, string title = "Title");

    /// <summary>
    /// Shows a confirmation dialog with "OK" and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if "OK" was chosen; otherwise <see langword="false"/>.</returns>
    Task<bool> ShowOkAbortAsync(string message, string title = "Title");

    /// <summary>
    /// Shows a confirmation dialog with "Yes", "No", and "Cancel" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns>
    /// The index of the button chosen. The exact Yes/No/Cancel-to-index
    /// mapping is defined by the platform implementation of this interface —
    /// check the concrete adapter (e.g. the Avalonia/MAUI implementation)
    /// for the authoritative mapping before relying on specific values here.
    /// </returns>
    Task<int> ShowYesNoCancelAsync(string message, string title = "Title");

    /// <summary>
    /// Shows a confirmation dialog with "Yes", "No", and "Abort" buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns>
    /// The index of the button chosen. The exact Yes/No/Abort-to-index
    /// mapping is defined by the platform implementation of this interface —
    /// check the concrete adapter (e.g. the Avalonia/MAUI implementation)
    /// for the authoritative mapping before relying on specific values here.
    /// </returns>
    Task<int> ShowYesNoAbortAsync(string message, string title = "Title");

    /// <summary>
    /// Shows <typeparamref name="TViewModel"/> as a modal dialog and awaits
    /// its result.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The ViewModel type to host in the dialog. Must implement
    /// <see cref="ICloseableViewModel{TResult}"/> so the dialog knows how
    /// to close itself and produce a result.
    /// </typeparam>
    /// <typeparam name="TResult">The type of value the ViewModel closes with.</typeparam>
    /// <param name="context">
    /// Optional navigation context passed through to the ViewModel/view
    /// resolution, e.g. for parameterizing which instance/data the dialog
    /// should operate on. If <see langword="null"/>, a default context is used.
    /// </param>
    /// <returns>
    /// The result the ViewModel closed with, or <see langword="null"/> if
    /// the dialog was dismissed without producing a result (e.g. cancelled).
    /// </returns>
    Task<TResult?> ShowModalAsync<TViewModel, TResult>(NavigableContext? context = null)
        where TViewModel : class, ICloseableViewModel<TResult>;
}