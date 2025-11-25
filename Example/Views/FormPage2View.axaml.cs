using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormPage2View : UserControl, IContextFor<FormPage2ViewModel>
{
    public FormPage2View()
    {
        InitializeComponent();
    }
}