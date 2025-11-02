namespace Avae.Abstractions
{
    public interface ITaskDialogService
    {
        Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results);
    }

    public class TaskDialogParams
    {
        public string? Title { get; set; }
        public string? Header { get; set; }
        public string? SubHeader { get; set; }
        public object? Content { get; set; }
        public object? IconSource { get; set; }
        public bool ShowProgressBar { get; set; }
        public TaskDialogFooterVisibility FooterVisibility { get; set; }
        public bool IsFooterExpanded { get; set; }
        public object? Footer { get; set; }

        public object? XamlRoot { get; set; }

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
        public Func<bool>? Closing;

        /// <summary>
        /// Occurs after the dialog is closed.
        /// </summary>
        public Action? Closed;
    }
    public enum TaskDialogFooterVisibility
    {
        //
        // Summary:
        //     The footer is never shown
        Never,
        //
        // Summary:
        //     The footer is hidden by default, but can be expanded open
        Auto,
        //
        // Summary:
        //     The footer is always visible
        Always
    }

    /// <summary>
    /// Defines constants that for standardized results from a <see cref="TaskDialog"/>
    /// </summary>
    public enum TaskDialogStandardResult
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        OK,

        /// <summary>
        /// 
        /// </summary>
        Cancel,

        /// <summary>
        /// 
        /// </summary>
        Yes,

        /// <summary>
        /// 
        /// </summary>
        No,

        /// <summary>
        /// 
        /// </summary>
        Retry,

        /// <summary>
        /// 
        /// </summary>
        Close
    }

}
