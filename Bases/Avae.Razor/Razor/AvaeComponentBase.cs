using Microsoft.AspNetCore.Components;

namespace Avae.Razor;

public partial class AvaeComponentBase<TViewModel>
    : ComponentBase where TViewModel : class
{
    [Inject]
    public required MudBlazor.ISnackbar Snackbar { get; set; }

    [Inject]
    public required MudBlazor.IDialogService MudDialogService { get; set; }

    [Inject]
    public required IServiceProvider Provider { get; set; }

    [Inject]
    public required ICircuitProvider CircuitProvider { get; set; }

    [Parameter, EditorRequired]
    public required TViewModel ViewModel { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        TaskDialogService.MudDialogService = MudDialogService;
        ContentDialogService.MudDialogService = MudDialogService;
        NotificationService.SnackbarService = Snackbar;
        DialogService.MudDialogService = MudDialogService;
        ModalService.MudDialogService = MudDialogService;
        CircuitProvider.Provider = Provider;
    }
}
