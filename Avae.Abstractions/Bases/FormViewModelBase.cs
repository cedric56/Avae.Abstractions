using Avae.Abstractions.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avae.Abstractions
{
    public abstract partial class FormViewModelBase<TResult>(Router router) : PagesViewModelBase(router), ICloseableViewModel<TResult>
    {
        public event EventHandler<TResult?>? CloseRequested;

        public abstract string Title { get; }

        protected virtual Task<bool> CanClose() => Task.FromResult(true);

        private ICommand? closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                return closeCommand ??= new AsyncRelayCommand(async () =>
                {
                    if (await CanClose())
                        await Close(default);
                });
            }
        }

        public virtual ObservableCollection<NamedCommand> Commands => [new() { Command = CloseCommand, Name = "Close" }];

        public Task Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }
    }
}
