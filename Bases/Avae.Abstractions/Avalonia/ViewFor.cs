using Avae.ViewModels;
using Avalonia.Controls;

namespace Avae.Abstractions;

public class ViewFor<TViewModel> : UserControl, IViewFor<TViewModel>
{
    public object? Context { get => DataContext; set => DataContext = value; }
}
