using Avae.Abstractions.Commands;
using System.Windows.Input;

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

        private ICommand? closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand = new AsyncRelayCommand(async () =>
                {
                    if (await CanClose())
                        await Close(default);
                }));
            }
        }

        public Task Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }
    }
}
