using Avae.Services;

namespace Avae.Maui;

internal class ContentDialogService(IDialogService service) : IContentDialogService
{
    /// <summary>
    /// Displays a content dialog with up to three buttons (primary, secondary, close) and awaits the user's selection.
    /// </summary>
    /// <param name="params">The content dialog's configuration, including title, content, and button text.</param>
    /// <returns>The <see cref="ContentDialogResult"/> corresponding to the button the user selected.</returns>
    public async Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
    {
        return await ((DialogService)service).DisplayThreeButtons<ContentDialogResult>(
            @params.Title,
            @params.Content,
            @params.PrimaryButtonText,
            @params.SecondaryButtonText,
            @params.CloseButtonText,
            ContentDialogResult.Primary,
            ContentDialogResult.Secondary,
            ContentDialogResult.None);
    }
}
