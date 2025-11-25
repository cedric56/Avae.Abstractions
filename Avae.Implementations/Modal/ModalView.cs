using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.ViewModels;

namespace Avae.Implementations
{
    internal partial class ModalView<T, TResult> : UserControl, IFullApi<TResult>, ISetCloseAction
        where T : ICloseableViewModel<TResult>
    {
        public ModalView(ModalViewModel<T, TResult> viewModel)
        {
            DataContext = viewModel;
            EventHandler<TResult>? closeRequested = null!;
            viewModel.ViewModel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewModel.ViewModel.CloseRequested -= closeRequested;
                SetButtonResult(e);

                if (this.Parent is Window window)
                    window.Close();
                else
                    Close();
            };
        }

        protected override Type StyleKeyOverride => typeof(DialogViewBase);

        private TResult _buttonResult =  default;
        private Action _closeAction= () => { };

        public void SetButtonResult(TResult bdName)
        {
            _buttonResult = bdName;
        }

        public TResult GetButtonResult()
        {
            return _buttonResult;
        }

        public Task Copy()
        {
            var text = (DataContext as AbstractMsBoxViewModel)?.ContentMessage;
            return Clipboard.SetTextAsync(text);
        }

        public void Close()
        {
            _closeAction?.Invoke();
        }

        public void CloseWindow(object sender, EventArgs eventArgs)
        {
            ((IClose)this).Close();
        }

        public void SetCloseAction(Action closeAction)
        {
            _closeAction = closeAction;
        }
    }
}
