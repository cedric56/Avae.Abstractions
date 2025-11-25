using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia;

namespace Avae.Implementations;

public abstract class DialogViewBase : UserControl
{

}

public class DialogView<TViewModel, TResult> : DialogViewBase,
    IModalFor<TViewModel, TResult>
    where TViewModel : class, ICloseableViewModel<TResult>
{
    protected virtual string Title { get; } = string.Empty;
    protected virtual string Buttons { get; } = "Ok";
    protected virtual string Icon { get; } = "";
    protected TViewModel? ViewModel { get { return DataContext as TViewModel; } }
    protected virtual eTypeDialog TypeDialog { get; } = eTypeDialog.Box;

    private ContentDialogParams CreateContentDialogParams(ModalParameters<TViewModel, TResult> parameters)
    {
        ContentDialogParams? @params = null;

        @params = new ContentDialogParams
        {
            Content = this,
            Title = Title,            
            PrimaryButtonText = parameters.Definitions.ElementAt(0).Name,
            PrimaryButtonCommand = parameters.Definitions.ElementAt(0).Command,
            SecondaryButtonText = parameters.Definitions.ElementAtOrDefault(1)?.Name,
            SecondaryButtonCommand = parameters.Definitions.ElementAtOrDefault(1)?.Command,
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
            @params.CloseButtonText = parameters.Definitions.LastOrDefault()?.Name;
            @params.CloseButtonCommand = parameters.Definitions.LastOrDefault()?.Command;
        }

        return @params;
    }

    public async Task<TResult?> ShowDialogAsync()
    {
        TResult? result = default;
        var viewModel = ViewModel;
        if(viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var modalParams = new ModalParameters<TViewModel, TResult>(Icon, Buttons, viewModel)
        {
            Content = this,
            ContentTitle = Title,
            CloseOnClickAway = true
        };

        if (TypeDialog == eTypeDialog.Fluent)
        {
            var contentDialogParams = CreateContentDialogParams(modalParams);
            var _contentDialogService = SimpleProvider.GetService<IContentDialogService>();            

            EventHandler<TResult>? closeRequested = null!;
            viewModel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewModel.CloseRequested -= closeRequested;
                result = e;
            };

            await _contentDialogService?.ShowAsync(contentDialogParams);
        }
        else
        {
            var modalViewModel = new ModalViewModel<TViewModel, TResult>(modalParams, viewModel);
            var modalView = new ModalView<TViewModel, TResult>(modalViewModel);
            var box = new MsBox<ModalView<TViewModel, TResult>, ModalViewModel<TViewModel, TResult>, TResult>(modalView, modalViewModel);
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                result = await box.ShowWindowDialogAsync((Window)TopLevelStateManager.GetActive());
            else
                result = await box.ShowAsync();
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
