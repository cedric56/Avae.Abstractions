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
        public event EventHandler<TResult>? CloseRequested;

        protected virtual Task<bool> CanClose() => Task.FromResult(true);


        [RelayCommand]
        public async Task Close()
        {
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
            if (await CanClose())
                await Close(default);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.
        }

        public Task Close(TResult value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }
    }
}
