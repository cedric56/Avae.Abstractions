using Avae.ViewModels;

namespace Avae.Maui;

public partial class ViewFor : ContentView, IViewFor
{
    public object? Context
    {
        get => BindingContext;
        set
        {
            if (Dispatcher.IsDispatchRequired)
                Dispatcher.Dispatch(() => BindingContext = value);
            else
                BindingContext = value;
        }
    }
}
