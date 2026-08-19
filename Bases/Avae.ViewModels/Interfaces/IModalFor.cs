namespace Avae.ViewModels;

public interface IModalFor<T, TResult> : IViewFor<T> where T : ICloseableViewModel<TResult>
{
    Task<TResult?> ShowModalAsync()
    {
        throw new NotImplementedException();
    }
}
