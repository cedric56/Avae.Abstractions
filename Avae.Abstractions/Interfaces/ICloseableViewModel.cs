namespace Avae.Abstractions;


public interface ICloseableViewModel<TResult> : IViewModelBase
{    
    event EventHandler<TResult?>? CloseRequested;
}
