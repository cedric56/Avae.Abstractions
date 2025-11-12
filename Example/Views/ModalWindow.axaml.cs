using Avae.Implementations;
using Example.ViewModels;

namespace Example;

public partial class ModalWindow : 
    DialogView<ModalViewModel,string>    
{
    protected override bool IsStandard =>  true;
    protected override string Buttons => "Validate,Cancel";

    public ModalWindow()
    {
        InitializeComponent();
    }
}