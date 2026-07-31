using Avae.Abstractions;

namespace Avae.Maui
{
    public interface IDialogView<TViewModel, TResult> : IModalFor<TViewModel, TResult>
     where TViewModel : ICloseableViewModel<TResult>
    {
        string Title { get; }
    }
}
