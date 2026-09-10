using Avae.ViewModels;

namespace Avae.Avalonia;

/// <summary>
/// MAUI implementation of view/modal resolution and the various dialog, notification, and theme
/// services, backed by an <see cref="IocContainer"/> and platform-specific dialog APIs.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve view models.</param>
/// <param name="getContainer">A factory that lazily supplies the <see cref="IocContainer"/> used for view resolution.</param>
/// <param name="configure">Optional callback invoked from <see cref="Configure(IIocContainer)"/> to register additional views/components.</param>
internal class IocConfiguration(Func<IocContainer> getContainer, Action<IIocContainer>? configure = null) : IIocConfiguration
{
    IocContainer? _container = null;

    /// <summary>
    /// Gets the IoC container used for view resolution, lazily created via <c>getContainer</c> on first access.
    /// </summary>
    IocContainer Container { get => _container ??= getContainer(); }

    /// <summary>
    /// Invokes the configured <c>configure</c> callback, if any, to register additional views/components with the container.
    /// </summary>
    /// <param name="container">The container to configure.</param>
    public void Configure(IIocContainer container)
    {
        configure?.Invoke(container);
    }

    /// <summary>
    /// Resolves and creates the view registered under the specified key.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="params">Additional arguments passed through to the view's registered factory.</param>
    /// <returns>The created view instance.</returns>
    public object? GetView(string key, params object[] @params)
    {
        return Container.GetView(key, @params);
    }

    /// <summary>
    /// Resolves the view registered under the specified key, using the supplied navigation context.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resolved view does not implement <see cref="IViewFor"/>.</exception>
    public IViewFor? GetContextFor(string key, NavigableContext context)
    {
        var view = Container.GetView(key, [context]);
        return view as IViewFor ?? throw new InvalidOperationException($"View must implement {nameof(IViewFor)}");
    }

    /// <summary>
    /// Resolves the strongly typed view for the specified view model type, using the supplied navigation context.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type whose view should be resolved.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor{TViewModel}"/>, or <see langword="null"/> if resolution fails or the result does not match.</returns>
    public IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigableContext context) where TViewModel : IViewModelBase
    {
        return Container.GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
    }

    /// <summary>
    /// Resolves the modal view associated with the specified closeable view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type whose modal view should be resolved.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved modal view, or <see langword="null"/> if resolution fails.</returns>
    public IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigableContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        return Container.GetModal<TViewModel, TResult>(context);
    }
}