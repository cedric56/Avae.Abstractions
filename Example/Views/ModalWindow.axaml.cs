using Avae.Abstractions;
using Avae.Services;
using Avalonia.Controls;
using Example.ViewModels;
using System;
using System.Threading.Tasks;

namespace Example;

public partial class ModalWindow : Window, IModalFor<ModalViewModel>
{
    public ModalWindow()
    {
        InitializeComponent();

        var viewModel = SimpleProvider.GetViewModel<ModalViewModel>();
        EventHandler<string>? closeRequested = null!;
        viewModel.CloseRequested += closeRequested = (sender, e) =>
        {
            viewModel.CloseRequested -= closeRequested;
            Close(e);
        };

        DataContext = viewModel;
    }

    public Task<TResult> ShowDialogAsync<TResult>()
    {
        return ShowDialog<TResult>((Window)TopLevelStateManager.GetActive());
    }
}