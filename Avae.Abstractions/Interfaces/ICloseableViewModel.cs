namespace Avae.Abstractions;

public interface ICloseableViewModel<TResult> : IViewModelBase
{
    public event EventHandler<TResult?>? CloseRequested;
}
