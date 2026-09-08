using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Avae.ViewModels;

/// <summary>
/// A simple container for registering and accessing pages in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public class IocContainer : IIocContainer, IIocConfiguration
{
    private IServiceProvider provider;

    /// <summary>
    /// Registered view factories, keyed by view identifier (typically a type name).
    /// </summary>
    private readonly ConcurrentDictionary<string, ViewFactory> _factories = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IocContainer"/> class, building and configuring
    /// a service provider from the supplied configuration.
    /// </summary>
    /// <param name="config">The configuration used to register services and views with this container.</param>
    /// <param name="buildServiceProvider">
    /// If <see langword="true"/>, a <see cref="ServiceProvider"/> is built from the configured services
    /// and assigned as this container's <see cref="Provider"/>. If <see langword="false"/>, the provider
    /// must be supplied later via <see cref="SetProvider"/>.
    /// </param>
    public IocContainer(IServiceProvider provider)
    {
        this.provider = provider;
    }

    /// <summary>
    /// Resolves and creates the view registered under the specified key.
    /// </summary>
    /// <param name="key">The key the view was registered under, typically a type name.</param>
    /// <param name="context">Additional arguments passed through to the view's registered factory.</param>
    /// <returns>The created view instance.</returns>
    /// <exception cref="Exception">Thrown if no view is registered under <paramref name="key"/>.</exception>
    public object GetView(string key, object[] context)
    {
        if (_factories.TryGetValue(key, out var factory))
        {
            return factory(provider, context);
        }

        throw new Exception($"No such view registered: {key}");
    }

    /// <summary>
    /// Resolves the modal view associated with the view model type <typeparamref name="T"/>, verifying that
    /// its declared result type matches <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="T">The closeable view model type whose modal view should be resolved.</typeparam>
    /// <typeparam name="TResult">The expected result type produced when the modal is closed.</typeparam>
    /// <param name="context">The navigation context passed to the view's factory.</param>
    /// <returns>The resolved modal view, implementing <see cref="IModalFor{T, TResult}"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the resolved view's declared result type does not match <typeparamref name="TResult"/>,
    /// or if the resolved view does not implement <see cref="IModalFor{T, TResult}"/>.
    /// </exception>
    public IModalFor<T, TResult> GetModal<T, TResult>(NavigableContext context) where T : ICloseableViewModel<TResult>
    {
        var view = GetView(typeof(T).Name, [context]);

        // Now verify the TResult type matches
        Type viewType = view.GetType();
        Type[] interfaces = viewType.GetInterfaces();

        // Find the IModalFor<,> interface implementation
        var modalInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

        if (modalInterface != null)
        {
            Type[] genericArgs = modalInterface.GetGenericArguments();
            Type viewModelType = genericArgs[0];
            Type resultType = genericArgs[1];

            // Check if the TResult matches
            if (resultType != typeof(TResult))
            {
                throw new InvalidOperationException(
                    $"The view associated with view model {typeof(T).Name} expects result type {resultType.Name}, " +
                    $"but {typeof(TResult).Name} was requested.");
            }
        }

        return view as IModalFor<T, TResult> ?? throw new InvalidOperationException($"The view associated with the view model {typeof(T).Name} is not a modal view.");
    }

    /// <summary>
    /// Registers a raw view factory under the specified key.
    /// </summary>
    /// <param name="key">The key to register the factory under, typically a type name.</param>
    /// <param name="factory">
    /// A factory that creates the view given a service provider and an array of context arguments.
    /// </param>
    public void Register(string key, Func<IServiceProvider, object[], object> factory)
    {
        _factories[key] = new ViewFactory(factory);
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/>, keyed by that type's name,
    /// which receives a strongly typed <see cref="NavigableContext"/>.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <param name="factory">A factory that creates the view given a service provider and a navigable context.</param>
    public void Register<TContextFor>(Func<IServiceProvider, NavigableContext, TContextFor> factory) where TContextFor : IViewFor
    {
        _factories[TContextFor.Name] = new ViewFactory((sp, args) => factory.Invoke(sp, (NavigableContext)args[0]));
    }

    /// <summary>
    /// Registers a view factory keyed by the name of <typeparamref name="T"/>, which receives a
    /// strongly typed <see cref="NavigableContext"/>.
    /// </summary>
    /// <typeparam name="T">The type whose name the factory is registered under.</typeparam>
    /// <param name="factory">A factory that creates the view given a service provider and a navigable context.</param>
    public void Register<T>(Func<IServiceProvider, NavigableContext, object> factory)
    {
        _factories[typeof(T).Name] = new ViewFactory((sp, args) => factory.Invoke(sp, (NavigableContext)args[0]));
    }

    /// <summary>
    /// Registers <typeparamref name="TContextFor"/> using its parameterless constructor as the factory.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered, which must have a public parameterless constructor.</typeparam>
    public void Register<TContextFor>() where TContextFor : IViewFor, new()
    {
        Register((sp, args) => new TContextFor());
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/> that takes a single
    /// positional argument in addition to the service provider.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <typeparam name="TArg1">The type of the first positional argument.</typeparam>
    /// <param name="func">A factory that creates the view given a service provider and the first argument.</param>
    public void Register<TContextFor, TArg1>(Func<IServiceProvider, TArg1, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, args.Get<TArg1>(0)));
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/> that takes two
    /// positional arguments in addition to the service provider.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <typeparam name="TArg1">The type of the first positional argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second positional argument.</typeparam>
    /// <param name="func">A factory that creates the view given a service provider and the two arguments.</param>
    public void Register<TContextFor, TArg1, TArg2>(Func<IServiceProvider, TArg1, TArg2, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, args.Get<TArg1>(0), args.Get<TArg2>(1)));
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/> that takes three
    /// positional arguments in addition to the service provider.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <typeparam name="TArg1">The type of the first positional argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second positional argument.</typeparam>
    /// <typeparam name="TArgs3">The type of the third positional argument.</typeparam>
    /// <param name="func">A factory that creates the view given a service provider and the three arguments.</param>
    public void Register<TContextFor, TArg1, TArg2, TArgs3>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, args.Get<TArg1>(0), args.Get<TArg2>(1), args.Get<TArgs3>(2)));
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/> that takes four
    /// positional arguments in addition to the service provider.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <typeparam name="TArg1">The type of the first positional argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second positional argument.</typeparam>
    /// <typeparam name="TArgs3">The type of the third positional argument.</typeparam>
    /// <typeparam name="TArgs4">The type of the fourth positional argument.</typeparam>
    /// <param name="func">A factory that creates the view given a service provider and the four arguments.</param>
    public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, args.Get<TArg1>(0), args.Get<TArg2>(1), args.Get<TArgs3>(2), args.Get<TArgs4>(3)));
    }

    /// <summary>
    /// Registers a view factory for <typeparamref name="TContextFor"/> that takes five
    /// positional arguments in addition to the service provider.
    /// </summary>
    /// <typeparam name="TContextFor">The view type being registered.</typeparam>
    /// <typeparam name="TArg1">The type of the first positional argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second positional argument.</typeparam>
    /// <typeparam name="TArgs3">The type of the third positional argument.</typeparam>
    /// <typeparam name="TArgs4">The type of the fourth positional argument.</typeparam>
    /// <typeparam name="TArgs5">The type of the fifth positional argument.</typeparam>
    /// <param name="func">A factory that creates the view given a service provider and the five arguments.</param>
    public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TArgs5, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, args.Get<TArg1>(0), args.Get<TArg2>(1), args.Get<TArgs3>(2), args.Get<TArgs4>(3), args.Get<TArgs5>(4)));
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
        var view = GetView(key, [context]);
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
        return GetView(typeof(TViewModel).Name, [context]) as IViewFor<TViewModel>;
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
        return GetModal<TViewModel, TResult>(context);
    }
}