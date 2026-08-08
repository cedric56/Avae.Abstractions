using Avae.Abstractions;
using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.ViewModels;

namespace Avae.Implementations;

internal abstract class ModalViewModelBase(ModalParameters parameters) : 
    MsBoxCustomViewModel(parameters)
{
    public IEnumerable<ModalButton> Definitions
    {
        get
        {
            return ButtonDefinitions.Cast<ModalButton>();
        }
    }
    protected ModalParameters Parameters { get; } = parameters;
    public object? ViewModel { get;  }
    public UserControl? Content { get; } = parameters.Content;
}

internal class ModalViewModel<T, TResult>(ModalParameters<T,TResult> parameters, T viewModel) :
    ModalViewModelBase(parameters), ISetFullApi<TResult>
    where T : ICloseableViewModel<TResult>
{
    public new T ViewModel { get; } = viewModel;

    public void SetFullApi(IFullApi<TResult> fullApi)
    {

    }
}
