using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avae.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CloseableAttribute(int index) : Attribute
    {
        public int Index { get; } = index;
    }

    public partial class CloseableViewModelBase<TResult> : ObservableObject, ICloseableViewModel<TResult>
    {
        public event EventHandler<TResult?>? CloseRequested;

        [RelayCommand]
        public async Task Close()
        {
            await Close(default);
        }

        public Task Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }
    }
}
