using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormPage1View : UserControl, IContextFor<FormPage1ViewModel>
{
    public FormPage1View()
    {
        InitializeComponent();
    }
}