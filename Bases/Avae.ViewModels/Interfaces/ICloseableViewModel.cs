using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Avae.ViewModels;
public class NamedCommand
{
    public required string Name { get; set; }
    public required ICommand Command { get; set; }
}

public interface ICloseableViewModel<TResult> : IViewModelBase
{
    string Title { get; }
    ObservableCollection<NamedCommand> Commands { get; }
    ICommand? CloseCommand { get; }
    event EventHandler<TResult?>? CloseRequested;
    Task Close(TResult? value);
}


public interface IViewModelErrorInfo : IDataErrorInfo
{
    void RaiseErrorChanged();
}