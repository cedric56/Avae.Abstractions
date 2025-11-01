using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace Example
{
    public interface IDialogService
    {
        Task<bool> ShowDialog(string message);
    }

    public class DialogService : IDialogService
    {
        public async Task<bool> ShowDialog(string message)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MsBox.Avalonia.Dto.MessageBoxStandardParams()
            {
                 MinWidth  = 400,
                SizeToContent = SizeToContent.WidthAndHeight,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.YesNo,
                 WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            return ButtonResult.Yes == await box.ShowWindowDialogAsync((Window)TopLevelStateManager.GetActive());
        }
    }

    public static class DialogWrapper
    {
        static IDialogService? instance = SimpleProvider.GetService<IDialogService>();

        public static Task<bool> ShowDialog(string message)
        {
            return instance?.ShowDialog(message) ?? Task.FromResult(false);
        }
    }
}
