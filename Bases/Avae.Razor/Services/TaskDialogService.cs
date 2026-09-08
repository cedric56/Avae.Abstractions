using Avae.Razor.Components;
using Avae.Services;

namespace Avae.Razor.Services;

internal class TaskDialogService : ITaskDialogService
{
    public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;
    public async Task<TaskDialogStandardResult> ShowAsync(TaskDialogParams @params, params TaskDialogStandardResult[] results)
    {
        var dialog = await MudDialogService.ShowAsync<TaskDialog>(@params.Title,
        new MudBlazor.DialogOptions()
        {
            BackdropClick = true
        });
        var result = await dialog.Result;
        return result?.Data is TaskDialogStandardResult cdr ? cdr : TaskDialogStandardResult.None;
    }
}
