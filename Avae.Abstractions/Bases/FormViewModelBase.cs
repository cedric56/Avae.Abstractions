using CommunityToolkit.Mvvm.Input;

namespace Avae.Abstractions
{
    public abstract partial class FormViewModelBase : PagesViewModelBase, ICloseableViewModel<bool>
    {
        public FormViewModelBase(Router router)
            : base(router)
        {

        }

        public event EventHandler<bool>? CloseRequested;

        public abstract string Title { get; }

        protected virtual Task<bool> CanClose() => Task.FromResult(true);

        [RelayCommand]
        public Task Close()
        {
            return Close(default);
        }

        public async Task Close(bool value)
        {
            if (await CanClose())
                CloseRequested?.Invoke(this, value);
        }
    }
}
