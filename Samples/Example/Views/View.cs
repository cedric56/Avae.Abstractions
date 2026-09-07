using Avae.ViewModels;
using Avalonia.Controls;

namespace Example;

public class View : UserControl, IViewFor
{
    public object? Context { get => DataContext; set => DataContext = value; }
}
