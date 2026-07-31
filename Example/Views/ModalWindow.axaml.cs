using Avae.Abstractions;
using Avae.Implementations;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel,string?>    
{
    protected override TypeDialog TypeDialog => TypeDialog.Fluent;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";
    protected override string Title => "Modal";

    public ModalWindow()
    {
        InitializeComponent();
    }
}