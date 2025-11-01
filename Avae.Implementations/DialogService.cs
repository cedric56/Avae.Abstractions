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

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            return ShowMessage(message, title, buttonText, afterHideCallback);
        }

        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            return ShowMessage(error.Message, title, buttonText, afterHideCallback);
        }

        public Task ShowMessage(string message, string title)
        {
            return ShowMessage(message, title, "Ok", afterHideCallback: null);
        }

        public async Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            var @params = GetParams(title, message, buttonText.ToEnum<ButtonEnum>());
            var result = await Show(@params);
            afterHideCallback?.Invoke();
        }

        public async Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool>? afterHideCallback)
        {
            var buttons = buttonConfirmText.ToEnum<ButtonEnum>();
            var confirmButton = buttonConfirmText.ToEnum<ButtonResult>();
            var @params = GetParams(title, message, buttons);
            var result = await Show(@params);
            afterHideCallback?.Invoke(result == confirmButton);
            var isConfirmed = result == confirmButton;
            afterHideCallback?.Invoke(isConfirmed);
            return isConfirmed;
        }

        public async Task<int> ShowMessage(string message, string title, string buttonText, Action<int>? callback)
        {
            var buttons = buttonText.ToEnum<ButtonEnum>();
            var splits = buttonText.SplitOnCapitals();
            var @params = GetParams(title, message, buttons);
            var result = await Show(@params);
            var possibilities = splits.Select(s => s.ToEnum<ButtonResult>()).ToList();
            var index = possibilities.IndexOf(result);
            callback?.Invoke(index);
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

        
    }
}
