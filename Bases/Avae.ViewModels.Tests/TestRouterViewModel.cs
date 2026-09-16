namespace Avae.ViewModels.Tests.TestDoubles;

/// <summary>
/// Concrete <see cref="RouterViewModelBase"/> that records when its protected hooks are invoked,
/// so tests can assert on <see cref="RouterViewModelBase.GoBack"/>/<see cref="RouterViewModelBase.GoForward"/>
/// behavior without depending on CommunityToolkit.Mvvm.
/// </summary>
public class TestRouterViewModel(Router router) : RouterViewModelBase(router)
{
    public int RaiseCanExecutesChangedCallCount { get; private set; }

    public IViewModelBase? LastViewModelChanged { get; private set; }

    protected override void RaiseCanExecutesChanged()
    {
        RaiseCanExecutesChangedCallCount++;
    }

    protected override void OnViewModelChanged(IViewModelBase viewModel)
    {
        LastViewModelChanged = viewModel;
        base.OnViewModelChanged(viewModel);
    }
}
