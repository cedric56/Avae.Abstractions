using System.Windows.Input;

namespace Avae.Abstractions
{
    public interface IContentDialogService
    {
        /// <summary>
        /// Begins an asynchronous operation to show the dialog.
        /// </summary>
        Task<ContentDialogResult> ShowAsync(ContentDialogParams @params);

        /// <summary>
        /// Begins an asynchronous operation to show the dialog using the specified window
        /// </summary>
        Task<ContentDialogResult> ShowAsync(object owner, ContentDialogParams @params);
    }
    public class ContentDialogParams
    {
        public string? Title { get; set; }
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
}
