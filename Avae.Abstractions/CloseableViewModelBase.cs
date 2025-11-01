using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avae.Abstractions
{
    public partial class CloseableViewModelBase : ObservableObject, ICloseableViewModel
    {
        public event EventHandler<bool>? CloseRequested;

        protected virtual Task<bool> CanClose() => Task.FromResult(true);

        [RelayCommand]
        public async Task Close()
        {
            if (await CanClose())
                CloseRequested?.Invoke(this, false);
        }
    }
}
