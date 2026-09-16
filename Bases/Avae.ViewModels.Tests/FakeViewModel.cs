namespace Avae.ViewModels.Tests.TestDoubles;

/// <summary>
/// A trivial <see cref="IViewModelBase"/> used as a navigation/DI target in tests.
/// </summary>
public class FakeViewModel : IViewModelBase
{
}

/// <summary>
/// A second, distinct <see cref="IViewModelBase"/> type, used where tests need two
/// different view model types (e.g. to exercise <see cref="NavigableViewModelBase"/> switching).
/// </summary>
public class OtherFakeViewModel : IViewModelBase
{
}
