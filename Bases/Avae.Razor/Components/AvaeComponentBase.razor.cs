using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Avae.Razor.Components
{
    public partial class AvaeComponentBase<TViewModel>
        : ComponentBase where TViewModel : class
    {
        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Inject]
        public required IDialogService DialogService { get; set; }

        [Parameter, EditorRequired]
        public required TViewModel ViewModel { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            TaskDialogService.MudDialogService = DialogService;
            ContentDialogService.MudDialogService = DialogService;
            NotificationService.SnackbarService = Snackbar;
            Avae.Razor.DialogService.MudDialogService = DialogService;
            ModalService.MudDialogService = DialogService;
        }
    }
}
