namespace Avae.Abstractions;

public interface IDialogService
{
    Task ShowErrorAsync(Exception ex, string title = "Error");
    Task ShowOkAsync(string message, string title = "Title");

    Task<bool> ShowYesNoAsync(string message, string title = "Title");

    Task<bool> ShowOkCancelAsync(string message, string title = "Title");

    Task<bool> ShowOkAbortAsync(string message, string title = "Title");

    Task<int> ShowYesNoCancelAsync(string message, string title = "Title");

    Task<int> ShowYesNoAbortAsync(string message, string title = "Title");
}

