using Avae.Implementations;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel,string>    
{
    protected override bool IsStandard =>  true;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";
    protected override string Buttons => "Validate,Cancel";
    protected override string Title => "Modal";

    public ModalWindow()
    {
        InitializeComponent();
    }
}