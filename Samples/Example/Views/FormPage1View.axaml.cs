using Avae.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Example.ViewModels;

namespace Example;

public partial class FormPage1View : UserControl, 
    IViewFor<FormViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    public FormPage1View()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        tb.Focus();
    }
}