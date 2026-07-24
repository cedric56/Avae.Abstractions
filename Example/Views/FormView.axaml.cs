using Avae.Abstractions;
using Avalonia.Controls;
using Example.ViewModels;

namespace Example;

public partial class FormView : UserControl, IContextFor<FormViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public FormView()
    {
        InitializeComponent();
    }
}