namespace Avae.Abstractions;

public interface ICloseableViewModel : IViewModelBase
{
    public event EventHandler<bool>? CloseRequested;
}
