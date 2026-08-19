using Avae.ViewModels;
using Avae.Core;
using Avae.Services;
using Avalonia.Controls;
using MsBox.Avalonia;

namespace Avae.Avalonia;

public abstract class DialogViewBase : UserControl
{

}

public class DialogView<TViewModel, TResult> : DialogViewBase,
    IModalFor<TViewModel, TResult?>
    where TViewModel : class, ICloseableViewModel<TResult?>
{
    public object? Context { get => DataContext; set => DataContext = value; }
    protected virtual string Icon { get; } = "";
    protected TViewModel? ViewModel { get { return DataContext as TViewModel; } }
    protected virtual TypeDialog TypeDialog { get; } = TypeDialog.Box;

    private ContentDialogParams CreateContentDialogParams(ModalParameters<TViewModel, TResult?> parameters)
    {
        ContentDialogParams? @params = null;

        @params = new ContentDialogParams
        {
            Content = this,
            Title = ViewModel?.Title,
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

        if (ViewModel?.Commands.Count > 2)
        {
            @params.CloseButtonText = parameters.Definitions.LastOrDefault()?.Name;
            @params.CloseButtonCommand = parameters.Definitions.LastOrDefault()?.Command;
        }

        return @params;
    }

    public async Task<TResult?> ShowModalAsync()
    {
        TResult? result = default;
        var viewModel = ViewModel;
        if(viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var modalParams = new ModalParameters<TViewModel, TResult?>(Icon, viewModel)
        {
            Content = this,
            ContentTitle = viewModel.Title,
            CloseOnClickAway = true
        };

        if (TypeDialog == TypeDialog.Fluent)
        {
            var contentDialogParams = CreateContentDialogParams(modalParams);
            var contentDialogService = ServiceLocator.GetRequiredService<IContentDialogService>();
            EventHandler<TResult>? closeRequested = null!;
            viewModel.CloseRequested += closeRequested = (sender, e) =>
            {
                viewModel.CloseRequested -= closeRequested;
                result = e;
            };

            await contentDialogService.ShowAsync(contentDialogParams);
        }
        else
        {
            var modalViewModel = new ModalViewModel<TViewModel, TResult?>(modalParams, viewModel);
            var modalView = new ModalView<TViewModel, TResult?>(modalViewModel);
            var box = new MsBox<ModalView<TViewModel, TResult?>, ModalViewModel<TViewModel, TResult?>, TResult?>(modalView, modalViewModel);
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                result = await box.ShowWindowDialogAsync((Window)TopLevelStateManager.Default.GetActive()!);
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
