using Avae.Avalonia;
using Avae.Services;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel, string?>
{
    protected override TypeDialog TypeDialog => TypeDialog.Fluent;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";

    public ModalWindow(IContentDialogService contentDialogService)
        : base(contentDialogService)
    {
        InitializeComponent();
    }
}