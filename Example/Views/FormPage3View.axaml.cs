using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormPage3View : UserControl, IContextFor<FormPage3ViewModel>
{
    public FormPage3View()
    {
        InitializeComponent();
    }
}