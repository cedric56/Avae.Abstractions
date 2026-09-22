using Avae.Avalonia;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel, string?>
{
    protected override bool IsFluent => true;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";

    public ModalWindow()
    {
        InitializeComponent();
    }
}