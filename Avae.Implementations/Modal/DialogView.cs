using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;

namespace Avae.Implementations;

public abstract class DialogViewBase : UserControl
{

}

public class DialogView<T, TResult> : DialogViewBase,
    IModalFor<T, TResult>
    where T : CloseableViewModelBase<TResult>
{
    protected virtual string Title { get; } = string.Empty;
    protected virtual string Buttons { get; } = "Ok";
    protected virtual string Icon { get; } = "";

    protected virtual bool IsStandard { get; } = true;


    private ContentDialogParams CreateContentDialogParams(ModalParameters<T, TResult> parameters)
    {
        ContentDialogParams? @params = null;

        @params = new ContentDialogParams
        {
            Content = this,
            Title = Title,            
            PrimaryButtonText = parameters.Dic.ElementAt(0).Key,
            PrimaryButtonCommand = parameters.Dic.ElementAt(0).Value,
            SecondaryButtonText = parameters.Dic.ElementAtOrDefault(1).Key,
            SecondaryButtonCommand = parameters.Dic.ElementAtOrDefault(1).Value,
            Closing = result =>
            {
                bool value = true;
                if (result == "Primary")
                    value = @params!.PrimaryButtonCommand?.CanExecute(@params.PrimaryButtonCommandParameter) ?? true;
                else if (result == "Secondary")
                    value = @params!.SecondaryButtonCommand?.CanExecute(@params.SecondaryButtonCommandParameter) ?? true;
                else 
                    value = @params!.CloseButtonCommand?.CanExecute(@params.CloseButtonCommandParameter) ?? true;

                return !value;
            }
        };

        if (Buttons.Split(",").Length > 2)
        {
            @params.CloseButtonText = parameters.Dic.LastOrDefault().Key;
            @params.CloseButtonCommand = parameters.Dic.LastOrDefault().Value;
        }

        return @params;
    }

    public async Task<TResult?> ShowDialogAsync()
    {
        TResult? result = default;
        var viewmodel = (T)DataContext;

        var modalParams = new ModalParameters<T, TResult>(Icon, Buttons, viewmodel)
        {
            Content = this,
            ContentTitle = Title,
            CloseOnClickAway = true
        };

        if (!IsStandard)
        {
            var contentDialogParams = CreateContentDialogParams(modalParams);
            var _contentDialogService = SimpleProvider.GetService<IContentDialogService>();            

            EventHandler<TResult>? closeRequested = null!;            
            viewmodel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewmodel.CloseRequested -= closeRequested;
                result = e;
            };

            await _contentDialogService?.ShowAsync(contentDialogParams);
        }
        else
        {
            var viewModel = new ModalViewModel<T, TResult>(modalParams, viewmodel);
            var box = new ModalView<T, TResult>(viewModel);
            var modal = new MsBox<ModalView<T, TResult>, ModalViewModel<T, TResult>, TResult>(box, viewModel);
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                result = await modal.ShowWindowDialogAsync((Window)TopLevelStateManager.GetActive());
            else
                result = await modal.ShowAsync();
        }

        await OnValidate(result);
        return result;
    }

    /// <summary>
    /// Override to perform custom validation logic if needed.
    /// </summary>
    protected virtual Task OnValidate(TResult? result)
    {
        return Task.CompletedTask;
    }
}
