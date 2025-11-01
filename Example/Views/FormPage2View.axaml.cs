using Avae.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Example.ViewModels;

namespace Example;

public partial class FormPage2View : UserControl, IContextFor<FormPage2ViewModel>
{
    public FormPage2View()
    {
        InitializeComponent();
    }
}