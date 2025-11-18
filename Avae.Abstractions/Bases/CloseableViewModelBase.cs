using Avae.Abstractions.Commands;
using System.Windows.Input;

namespace Avae.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CloseableAttribute(int index) : Attribute
    {
        public int Index { get; } = index;
    }

    public partial class CloseableViewModelBase<TResult> : ICloseableViewModel<TResult>
    {
        public event EventHandler<TResult?>? CloseRequested;

        private ICommand? closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand = new AsyncRelayCommand(() => Close(default)));
            }
        }

        public Task Close(TResult? value)
        {
            CloseRequested?.Invoke(this, value);
            return Task.CompletedTask;
        }

        public virtual Task OnLaunched()
        {
            return Task.CompletedTask;
        }
    }
}
