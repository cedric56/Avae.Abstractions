using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avae.Abstractions;
public class NamedCommand
{
    public required string Name { get; set; }
    public required ICommand Command { get; set; }
}

public interface ICloseableViewModel<TResult> : IViewModelBase
{
    ObservableCollection<NamedCommand> Commands { get; }
    ICommand? CloseCommand { get; }
    event EventHandler<TResult?>? CloseRequested;
    Task Close(TResult? value);
}
