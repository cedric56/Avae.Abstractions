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
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
            return Close(default);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.
        }

        public async Task Close(TResult value)
        {
            if (await CanClose())
                CloseRequested?.Invoke(this, value);
        }
    }
}
