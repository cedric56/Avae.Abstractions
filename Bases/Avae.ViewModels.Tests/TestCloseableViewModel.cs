using System.Windows.Input;

namespace Avae.ViewModels.Tests.TestDoubles;

/// <summary>
/// Concrete <see cref="CloseableViewModelBase{TResult}"/> used to test the base class's
/// close/command/title behavior in isolation.
/// </summary>
public class TestCloseableViewModel : CloseableViewModelBase<string>
{
    public bool CanCloseResult { get; set; } = true;

    public override ICommand CloseCommand { get; } //= new DelegateCommand(() => { });

    public override string Title => "Test Title";

    public override Task<bool> CanClose() => Task.FromResult(CanCloseResult);
}
