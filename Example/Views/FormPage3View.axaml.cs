using Avae.Abstractions;
using Avalonia.Controls;
using Example.Models;
using Example.ViewModels;

namespace Example;

public partial class FormPage3View : UserControl, IViewFor<FormPage3ViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public FormPage3View()
    {
        InitializeComponent();
    }

    public FormPage3View(Person person)
        : this()
    {

    }
}