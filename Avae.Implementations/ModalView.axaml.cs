using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace Avae.Implementations
{
    public partial class ModalView : UserControl, IFullApi<string>, ISetCloseAction
    {
        private string _buttonResult;
        private Action _closeAction;

        public ModalView()
        {
            InitializeComponent();
        }

        public void SetButtonResult(string bdName)
        {
            _buttonResult = bdName;
        }

        public string GetButtonResult()
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
            try
            {
                _closeAction?.Invoke();
            }
            catch
            {

            }

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
