using Avae.Abstractions;
using Avae.Implementations;

namespace Example;

public partial class ModalWindow : 
    DialogView<ViewModels.ModalViewModel,string>    
{
    protected override bool IsStandard =>  true;
    protected override string Buttons => "Validate,Cancel";

    public ModalWindow()
    {
        InitializeComponent();

        var viewModel = SimpleProvider.GetViewModel<ViewModels.ModalViewModel>();
        
        DataContext = viewModel;
    }
}