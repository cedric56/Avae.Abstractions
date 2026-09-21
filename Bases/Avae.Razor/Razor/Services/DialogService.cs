using Microsoft.AspNetCore.Components;
using IDialogService = Avae.Services.IDialogService;

namespace Avae.Razor;

internal class DialogService : IDialogService
{
    public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;    

    public async Task ShowErrorAsync(Exception ex, string title = "Error")
    {
        await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(ex.Message.Replace(Environment.NewLine, "<br/>")));
    }

    public async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
    {
        return await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
            cancelText: "Abort") ?? false;
    }

    public async Task ShowOkAsync(string message, string title = "Title")
    {
        await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")));
    }

    public async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
    {
        return await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
            cancelText: "Cancel") ?? false;
    }

    public async Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
    {
        var result = await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
            yesText: "Yes",
            noText: "No",
            cancelText: "Abort");
        return result switch
        {
            true => 0,
            false => 1,
            _ => 2
        };
    }

    public async Task<bool> ShowYesNoAsync(string message, string title = "Title")
    {
        return await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
            yesText: "Yes",
            cancelText: "No") ?? false;
    }

    public async Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
    {
        var result = await MudDialogService.ShowMessageBoxAsync(
            title,
            new MarkupString(message.Replace(Environment.NewLine, "<br/>")),
            yesText: "Yes",
            noText: "No",
            cancelText: "Cancel");
        return result switch
        {
            true => 0,
            false => 1,
            _ => 2
        };
    }
}
