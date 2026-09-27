using Avae.ViewModels;
using Example.ViewModels;

namespace Example.Maui;

internal class DefaultView : ContentView, IViewFor
{
    public object? Context
    {
        get => BindingContext;
        set
        {
            BindingContext = value;
            if (value is FormViewModel viewModel)
                this.Content = new Label() { Text = "Form" };
        }
    }
}

internal class DefaultPage : ContentPage, IViewFor
{
    public object? Context
    {
        get => BindingContext;
        set
        {
            BindingContext = value;
            if (value is RegionsViewModel viewModel)
                this.Content = new Label() { Text = "Regions" };
        }
    }
}
