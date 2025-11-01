using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avae.Abstractions
{
    public partial class CloseableViewModelBase<TResult> : ObservableObject, ICloseableViewModel<TResult>
    {
        public event EventHandler<TResult>? CloseRequested;

        protected virtual Task<bool> CanClose() => Task.FromResult(true);


        [RelayCommand]
        public Task Close()
        {
            return Close(default);
        }

        public async Task Close(TResult value)
        {
            if (await CanClose())
                CloseRequested?.Invoke(this, value);
        }
    }
}
