using Avae.ViewModels;
using Example.ViewModels;

namespace Example;

public partial class FormView : View, IViewFor<FormViewModel>
{
    public FormView()
    {
        InitializeComponent();
    }
}