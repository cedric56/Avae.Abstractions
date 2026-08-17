using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormView : UserControl, IViewFor<FormViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public FormView()
    {
        InitializeComponent();
    }
}