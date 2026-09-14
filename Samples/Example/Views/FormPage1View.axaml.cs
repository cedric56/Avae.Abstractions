using Avae.Abstractions;
using Avalonia.Interactivity;
using Example.ViewModels;

namespace Example;

public partial class FormPage1View : ViewFor<FormViewModel>
{
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