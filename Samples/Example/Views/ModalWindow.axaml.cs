using Avae.Avalonia;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel,string?>    
{
    protected override TypeDialog TypeDialog => TypeDialog.Fluent;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";
    
    public ModalWindow()
    {
        InitializeComponent();
    }
}