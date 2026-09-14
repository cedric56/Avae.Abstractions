using Avae.Services;
using System.Threading.Tasks;

namespace Avae.Abstractions;

internal class ContentDialogService : IContentDialogService
{
    public static MudBlazor.IDialogService MudDialogService { get; set; } = default!;
    public async Task<ContentDialogResult> ShowAsync(ContentDialogParams @params)
    {
        var dialog = await MudDialogService.ShowAsync<ContentDialog>(@params.Title,
        new MudBlazor.DialogParameters()
        {
        { "Parameters", @params }

        }, new MudBlazor.DialogOptions()
        {
            BackdropClick = true
        });
        var result = await dialog.Result;
        return result?.Data is ContentDialogResult cdr ? cdr : ContentDialogResult.None;
    }
}
