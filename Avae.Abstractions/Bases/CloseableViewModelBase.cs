using Avae.Abstractions.Commands;
using System.Windows.Input;

namespace Avae.Abstractions
{
    public partial class CloseableViewModelBase<TResult> : ICloseableViewModel<TResult>
    {
        public event EventHandler<TResult?>? CloseRequested;

        private ICommand? closeCommand;

        public ICommand? CloseCommand
        {
            get
            {
                return closeCommand ??= new AsyncRelayCommand(() => Close(default));
            }
        }

        public virtual CommandIndex[] Commands { get;  } = [];

        public Task Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }
    }
}
