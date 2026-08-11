using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormPage2View : UserControl, IViewFor<FormPage2ViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public FormPage2View()
    {
        InitializeComponent();
    }
}