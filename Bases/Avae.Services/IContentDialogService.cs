using System.Windows.Input;

namespace Avae.Services;

/// <summary>
/// Displays modal content-dialogs (a WinUI/Fluent-style "ContentDialog") in a
/// platform-agnostic way, so ViewModels can request a dialog without knowing
/// whether the app is running on Avalonia, MAUI, or Blazor.
/// </summary>
public interface IContentDialogService
{
    /// <summary>
    /// Begins an asynchronous operation to show the dialog.
    /// </summary>
    /// <param name="params">The content, buttons, and callbacks to show.</param>
    /// <returns>
    /// A task that completes with the <see cref="ContentDialogResult"/>
    /// corresponding to whichever button the user tapped (or
    /// <see cref="ContentDialogResult.None"/> if the dialog was dismissed
    /// without tapping a button, e.g. by pressing Escape or tapping outside).
    /// </returns>
    Task<ContentDialogResult> ShowAsync(ContentDialogParams @params);
}

/// <summary>
/// Describes the content, buttons, and lifecycle callbacks for a single
/// <see cref="IContentDialogService.ShowAsync"/> call. Mirrors the shape of
/// WinUI's ContentDialog so platform adapters can map these properties
/// directly onto their native dialog control.
/// </summary>
public class ContentDialogParams
{
    /// <summary>
    /// Gets or sets the title text shown at the top of the dialog.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the body content of the dialog. Typically a string,
    /// but platform adapters may also accept a view/control instance here
    /// for richer dialog bodies.
    /// </summary>
    public object? Content { get; set; }

    /// <summary>
    /// Gets or sets the command to invoke when the close button is tapped.
    /// </summary>
    public ICommand? CloseButtonCommand { get; set; }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the close button.
    /// </summary>
    public object? CloseButtonCommandParameter { get; set; }

    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string? CloseButtonText { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates which button on the dialog is the default action.
    /// </summary>
    // TODO: not yet wired up — intended to be a ContentDialogButton (see enum below)
    // once platform adapters support driving a default/focused button.
    //public ContentDialogButton

    /// <summary>
    /// Gets or sets whether the dialog's primary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether the dialog's secondary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled { get; set; }

    /// <summary>
    /// Gets or sets the command to invoke when the primary button is tapped.
    /// </summary>
    public ICommand? PrimaryButtonCommand { get; set; }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the primary button.
    /// </summary>
    public object? PrimaryButtonCommandParameter { get; set; }

    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string? PrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the command to invoke when the secondary button is tapped.
    /// </summary>
    public ICommand? SecondaryButtonCommand { get; set; }

    /// <summary>
    /// Gets or sets the parameter to pass to the command for the secondary button.
    /// </summary>
    public object? SecondaryButtonCommandParameter { get; set; }

    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string? SecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the title template.
    /// </summary>
    // TODO: not yet wired up — intended to let callers supply a data
    // template for Title instead of plain text, once platform adapters
    // expose an IDataTemplate-equivalent.
    //public IDataTemplate TitleTemplate
    //{
    //    get => GetValue(TitleTemplateProperty);
    //    set => SetValue(TitleTemplateProperty, value);
    //}

    /// <summary>
    /// Gets or sets whether the Dialog should show full screen
    /// On WinUI3, at least desktop, this just show the dialog at 
    /// the maximum size of a contentdialog.
    /// </summary>
    public bool FullSizeDesired { get; set; }

    /// <summary>
    /// Occurs before the dialog is opened
    /// </summary>
    public Action? Opening;

    /// <summary>
    /// Occurs after the dialog is opened.
    /// </summary>
    public Action? Opened;

    /// <summary>
    /// Occurs after the dialog starts to close, but before it is closed and before the Closed event occurs.
    /// </summary>
    /// <remarks>
    /// Return <see langword="false"/> to cancel the close and keep the
    /// dialog open; return <see langword="true"/> to allow it to proceed.
    /// The <see cref="string"/> argument identifies which button/action
    /// triggered the close (implementation-defined, e.g. "Primary",
    /// "Secondary", "Close", or "Dismiss").
    /// </remarks>
    public Func<string, bool>? Closing;

    /// <summary>
    /// Occurs after the dialog is closed.
    /// </summary>
    public Action? Closed;

    /// <summary>
    /// Occurs after the primary button has been tapped.
    /// </summary>
    public Action? PrimaryButtonClick;

    /// <summary>
    /// Occurs after the secondary button has been tapped.
    /// </summary>
    public Action? SecondaryButtonClick;

    /// <summary>
    /// Occurs after the close button has been tapped.
    /// </summary>
    public Action? CloseButtonClick;
}

/// <summary>
/// Specifies identifiers to indicate the return value of a ContentDialog
/// </summary>
public enum ContentDialogResult
{
    /// <summary>
    /// No button was tapped.
    /// </summary>
    None = 0,

    /// <summary>
    /// The primary button was tapped by the user.
    /// </summary>
    Primary = 1,

    /// <summary>
    /// The secondary button was tapped by the user.
    /// </summary>
    Secondary = 2
}

/// <summary>
/// Defines constants that specify the default button on a content dialog.
/// </summary>
// Not yet consumed by ContentDialogParams (see the commented-out
// "default action" property above) — reserved for when that's wired up.
public enum ContentDialogButton
{
    /// <summary>
    /// No button is specified as the default.
    /// </summary>
    None = 0,

    /// <summary>
    /// The primary button is the default.
    /// </summary>
    Primary = 1,

    /// <summary>
    /// The secondary button is the default.
    /// </summary>
    Secondary = 2,

    /// <summary>
    /// The close button is the default.
    /// </summary>
    Close = 3
}