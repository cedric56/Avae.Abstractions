namespace Avae.ViewModels.Tests.TestDoubles;

/// <summary>
/// A trivial <see cref="IViewFor"/> used to stand in for a resolved view in tests.
/// </summary>
public class FakeViewFor : IViewFor
{
    public object? Context { get; set; }
}

/// <summary>
/// A strongly typed <see cref="IViewFor{T}"/>, whose static <c>Name</c> resolves to
/// <c>typeof(T).Name</c> via the interface's default implementation - required for the
/// <see cref="IocContainer"/> overloads that register/resolve by view model type.
/// </summary>
public class FakeViewFor<T> : IViewFor<T>
{
    public object? Context { get; set; }
}

/// <summary>
/// A fake modal view used to test <see cref="IocContainer.GetModal{T, TResult}"/>.
/// </summary>
public class FakeModalFor<TViewModel, TResult> : IModalFor<TViewModel, TResult>
    where TViewModel : ICloseableViewModel<TResult>
{
    public object? Context { get; set; }
}
