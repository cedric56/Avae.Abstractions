namespace Avae.Abstractions;

public interface IModalFor<T, TResult> : IContextFor<T> where T : IViewModelBase
{
    Task<TResult?> ShowDialogAsync();
}
