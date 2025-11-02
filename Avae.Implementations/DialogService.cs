using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace Avae.Implementations
{
    public class DialogService(string iconUrl) : IDialogService
    {
        public string IconUrl { get; private set; } = iconUrl;


        public Task ShowSimpleMessage(string message, string title, string buttonText)
        {
            var @params = GetParams(title, message, buttonText.ToEnum<ButtonEnum>());
            return Show(@params);
        }

        public async Task<int> ShowMessage(string message, string title, string buttonText)
        {
            var buttons = buttonText.ToEnum<ButtonEnum>();
            var splits = buttonText.SplitOnCapitals();
            var @params = GetParams(title, message, buttons);
            var result = await Show(@params);
            var possibilities = splits.Select(s => s.ToEnum<ButtonResult>()).ToList();
            var index = possibilities.IndexOf(result);
            return index;
        }

        public async Task<ButtonResult> Show(MessageBoxStandardParams @params)
        {
            return await Dispatcher.UIThread.Invoke(async () =>
            {
                try
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(@params);
                    var owner = TopLevelStateManager.GetActive() as Window;
                    if (owner != null && (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
                    {
                        var r = await box.ShowWindowDialogAsync(owner);
                        return r;
                    }

                    var top = TopLevelStateManager.GetActive();
                    var result = top != null ? await box.ShowAsPopupAsync(top) : await box.ShowAsync();
                    return result;
                }
                catch
                {
                    return ButtonResult.None;
                }
            });
        }

        private MessageBoxStandardParams GetParams(string title, string message, ButtonEnum buttonEnum)
        {
            var @params = new MessageBoxStandardParams
            {
                Topmost = true,
                ButtonDefinitions = buttonEnum,
                ContentTitle = title,
                ContentMessage = message,
                WindowIcon = Extensions.GetIcon(IconUrl),
                MinWidth = 300,
                ShowInCenter = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,

            };
            return @params;
        }

        public Task ShowErrorAsync(Exception ex, string title = "Error")
        {
            return ShowSimpleMessage(ex.Message, title, "Ok");
        }

        public Task ShowOkAsync(string message, string title = "Title")
        {
            return ShowSimpleMessage(message, title, "Ok");
        }

        public async Task<bool> ShowYesNoAsync(string message, string title = "Title")
        {
            return await ShowMessage(message, title, "YesNo") == 0;
        }

        public async Task<bool> ShowOkCancelAsync(string message, string title = "Title")
        {
            return await ShowMessage(message, title, "OkCancel") == 0;
        }

        public async Task<bool> ShowOkAbortAsync(string message, string title = "Title")
        {
            return await ShowMessage(message, title, "OkAbort") == 0;
        }

        public Task<int> ShowYesNoCancelAsync(string message, string title = "Title")
        {
            return ShowMessage(message, title, "YesNoCancel");
        }

        public Task<int> ShowYesNoAbortAsync(string message, string title = "Title")
        {
            return ShowMessage(message, title, "YesNoAbort");
        }
    }
}
