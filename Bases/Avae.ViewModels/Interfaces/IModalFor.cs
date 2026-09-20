namespace Avae.ViewModels;

public interface IModalFor<T, TResult> : IContext where T : ICloseableViewModel<TResult>
{
    Task<TResult?> ShowModalAsync();
}