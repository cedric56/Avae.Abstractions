namespace Avae.Abstractions;

public interface IModalFor<T> : IContextFor<T> where T : IViewModelBase
{
    Task<TResult> ShowDialogAsync<TResult>();
}
