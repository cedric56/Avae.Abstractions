using Avae.Abstractions;
using Example.ViewModels;

namespace Example;

public partial class FormView : ViewFor<FormViewModel>
{
    public FormView()
    {
        InitializeComponent();
    }
}