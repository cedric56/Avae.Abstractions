namespace Avae.Abstractions;

public interface IModalFor<T, TResult> : IContextFor<T> where T : ICloseableViewModel<TResult>
{
    Task<TResult?> ShowModalAsync()
    {
        throw new NotImplementedException();
    }
}
