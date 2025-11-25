using System.Windows.Input;

namespace Avae.Abstractions;
public class CommandIndex
{
    public int? Index { get; set; }
    public ICommand? Command { get; set; }
}

public interface ICloseableViewModel<TResult> : IViewModelBase
{
    CommandIndex[] Commands { get; }
    ICommand? CloseCommand { get; }
    event EventHandler<TResult?>? CloseRequested;
    Task Close(TResult? value);
}
