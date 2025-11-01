using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormView : UserControl, IContextFor<FormViewModel>
{
    public FormView()
    {
        InitializeComponent();
    }
}