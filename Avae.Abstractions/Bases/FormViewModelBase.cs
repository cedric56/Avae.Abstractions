using CommunityToolkit.Mvvm.Input;

namespace Avae.Abstractions
{
    public abstract partial class FormViewModelBase<TResult> : PagesViewModelBase, ICloseableViewModel<TResult>
    {
        public FormViewModelBase(Router router)
            : base(router)
        {

        }

        public event EventHandler<TResult?>? CloseRequested;

        public abstract string Title { get; }

        protected virtual Task<bool> CanClose() => Task.FromResult(true);

        [RelayCommand]
        public async Task Close()
        {
            if (await CanClose())
                Close(default);
        }

        public void Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
        }
    }
}
