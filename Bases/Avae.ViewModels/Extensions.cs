using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Avae.ViewModels;

/// <summary>
/// General-purpose extension methods for resolving view models from an <see cref="IServiceProvider"/>
/// and synchronizing collections.
/// </summary>
public static class Extensions
{
    public static void Register(this IServiceCollection services, string key, Func<IServiceProvider, object[], object> factory,
          ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, factory);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => factory);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => factory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient)
    where TView : class where TViewModel : class
    {
        var key = typeof(TViewModel).Name;
        var map = ViewModelViewMap.Initialize(services);
        map.Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);

        switch (viewLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, func);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => func);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => func);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(viewLifetime), viewLifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient)
    where TView : class where TViewModel : class
    {
        var key = typeof(TViewModel).Name;
        var map = ViewModelViewMap.Initialize(services);
        map.Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);

        switch (viewLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, func);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => func);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => func);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(viewLifetime), viewLifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArgs3>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArgs3, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient)
    where TView : class where TViewModel : class
    {
        var key = typeof(TViewModel).Name;
        var map = ViewModelViewMap.Initialize(services);
        map.Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);

        switch (viewLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, func);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => func);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => func);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(viewLifetime), viewLifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArgs3, TArgs4>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient)
    where TView : class where TViewModel : class
    {
        var key = typeof(TViewModel).Name;
        var map = ViewModelViewMap.Initialize(services);
        map.Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);

        switch (viewLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, func);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => func);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => func);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(viewLifetime), viewLifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArgs3, TArgs4, TArgs5>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TArgs5, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient)
    where TView : class where TViewModel : class
    {
        var key = typeof(TViewModel).Name;
        var map = ViewModelViewMap.Initialize(services);
        map.Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);

        switch (viewLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton(key, func);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped(key, (provider, _) => func);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient(key, (provider, _) => func);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(viewLifetime), viewLifetime, null);
        }
    }

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
    this IServiceCollection services,
    ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
    ServiceLifetime viewLifetime = ServiceLifetime.Transient,
    string? key = null)
    where TView : class where TViewModel : class
    {
        key ??= typeof(TViewModel).Name;

        services.RegisterViewModel<TViewModel>(viewModelLifetime);
        services.RegisterView<TView>(key, viewLifetime);
    }

    /// <summary>
    /// Resolves or creates the view model of type <typeparamref name="T"/>, using a registered
    /// <see cref="ViewModelFactory{T}"/> if one exists, or falling back to direct service resolution.
    /// </summary>
    /// <typeparam name="T">The view model type to resolve.</typeparam>
    /// <param name="provider">The service provider to resolve from.</param>
    /// <param name="context">Optional navigation context supplying constructor parameters for the view model.</param>
    /// <returns>The resolved view model instance, cast to <typeparamref name="T"/>.</returns>
    public static T GetViewModel<T>(this IServiceProvider provider, NavigableContext? context = null)// where T : class, IViewModelBase
    {
        return (T)GetViewModel(provider, typeof(T), context);
    }

    static void RegisterView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView>(
   this IServiceCollection services,
   string key,
   ServiceLifetime lifetime = ServiceLifetime.Singleton) where TView : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton<Func<IServiceProvider, object[], object>>(
                    key,
                    ActivatorUtilities.CreateInstance<TView>);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped<Func<IServiceProvider, object[], object>>(
                    key,
                    (provider, _) => ActivatorUtilities.CreateInstance<TView>);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient<Func<IServiceProvider, object[], object>>(
                    key,
                    (provider, _) => ActivatorUtilities.CreateInstance<TView>);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
    }

    static void RegisterViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
    this IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Singleton) where TViewModel : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddKeyedSingleton<Func<IServiceProvider, object[], object>>(
                    typeof(TViewModel),
                    ActivatorUtilities.CreateInstance<TViewModel>);
                break;

            case ServiceLifetime.Scoped:
                services.AddKeyedScoped<Func<IServiceProvider, object[], object>>(
                    typeof(TViewModel),
                    (provider, _) => ActivatorUtilities.CreateInstance<TViewModel>);
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedTransient<Func<IServiceProvider, object[], object>>(
                    typeof(TViewModel),
                    (provider, _) => ActivatorUtilities.CreateInstance<TViewModel>);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
    }

    /// <summary>
    /// Resolves or creates a view model of the specified type, using a registered
    /// <see cref="ViewModelFactory{T}"/> if one exists, or falling back to direct service resolution.
    /// </summary>
    /// <param name="provider">The service provider to resolve from.</param>
    /// <param name="viewModelType">The type of view model to resolve.</param>
    /// <param name="context">Optional navigation context supplying constructor parameters for the view model.</param>
    /// <returns>The resolved view model instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a registered factory fails to create the view model; if constructor parameters are supplied
    /// via <paramref name="context"/> but no factory is registered for <paramref name="viewModelType"/>;
    /// or if <paramref name="viewModelType"/> cannot be resolved directly from <paramref name="provider"/>.
    /// </exception>
    public static object GetViewModel(this IServiceProvider provider, Type viewModelType, NavigableContext? context = null)
    {
        var factory = provider.GetKeyedService<Func<IServiceProvider, object[], object>>(viewModelType);
        if (factory is not null)
        {
            var viewModel = factory(provider, [.. context?.ViewModelParameters ?? []]);
            if (viewModel is not null)
                return viewModel;

            throw new InvalidOperationException($"Unable to create {viewModelType.Name}. Ensure that it is registered with the service provider.");
        }

        if (context?.ViewModelParameters.Length > 0)
        {
            throw new InvalidOperationException($"You must register using {nameof(RegisterViewModel)} for view models with parameters.");
        }

        if (provider.GetService(viewModelType) is object service)
        {
            return service;
        }

        throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
    }

    /// <summary>
    /// Resolves and creates the view registered under the specified key.
    /// </summary>
    /// <param name="key">The key the view was registered under, typically a type name.</param>
    /// <param name="context">Additional arguments passed through to the view's registered factory.</param>
    /// <returns>The created view instance.</returns>
    /// <exception cref="Exception">Thrown if no view is registered under <paramref name="key"/>.</exception>
    public static object GetView(this IServiceProvider provider, string key, object[] context)
    {
        var factory = provider.GetKeyedService<Func<IServiceProvider, object[], object>>(key);
        if (factory is not null)
        {
            var view = factory(provider, [.. context ?? []]);
            if (view is not null)
                return view;

            throw new InvalidOperationException($"Unable to create view for {key}. Ensure that it is registered with the service provider.");
        }

        throw new Exception($"No such view registered: {key}");
    }

    /// <summary>
    /// Resolves the view registered under the specified key, using the supplied navigation context.
    /// </summary>
    /// <param name="key">The key the view was registered under.</param>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resolved view does not implement <see cref="IViewFor"/>.</exception>
    public static IViewFor? GetContextFor(this IServiceProvider provider, string key, NavigableContext context)
    {
        if (context.ViewParameters.Any())
        {
            var map = provider.GetRequiredService<IViewModelViewMap>();
            var viewType = map.GetViewType(key);
            if (viewType != null)
            {
                return ActivatorUtilities.CreateInstance(provider, viewType, context.ViewParameters) as IViewFor;
            }
        }

        var factory = provider.GetKeyedService<Func<IServiceProvider, object[], object>>(
            context.FactoryParameters.ElementAtOrDefault(0) ??
            key);
        if (factory is not null)
        {
            var view = factory(provider, [.. context.ViewParameters ?? []]);
            if (view is not null)
                return view as IViewFor ?? throw new InvalidOperationException($"View must implement {nameof(IViewFor)}");

            throw new InvalidOperationException($"Unable to create view for {key}. Ensure that it is registered with the service provider.");
        }

        throw new Exception($"No such view registered: {key}");
    }

    /// <summary>
    /// Resolves the strongly typed view for the specified view model type, using the supplied navigation context.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type whose view should be resolved.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved view as an <see cref="IViewFor{TViewModel}"/>, or <see langword="null"/> if resolution fails or the result does not match.</returns>
    public static IViewFor<TViewModel>? GetContextFor<TViewModel>(this IServiceProvider provider,NavigableContext context)// where TViewModel : IViewModelBase
    {
        return provider.GetContextFor(typeof(TViewModel).Name, context) as IViewFor<TViewModel>;
    }

    /// <summary>
    /// Resolves the modal view associated with the specified closeable view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The closeable view model type whose modal view should be resolved.</typeparam>
    /// <typeparam name="TResult">The result type produced when the modal is closed.</typeparam>
    /// <param name="context">The navigation context to pass to the view's factory.</param>
    /// <returns>The resolved modal view, or <see langword="null"/> if resolution fails.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2075",
    Justification = "View types are always registered via explicit compile-time factories " +
                     "(e.g. new TContextFor(), ActivatorUtilities.CreateInstance<TContextFor>()) " +
                     "in IocContainer.Register<TContextFor>(...). Because the concrete type and its " +
                     "constructor are referenced directly at the registration call site, the trimmer " +
                     "already roots that type — including the interfaces it implements — as part of " +
                     "normal reachability analysis. This reflection only re-discovers metadata the " +
                     "trimmer has already preserved for an independent reason; it doesn't rely on " +
                     "trimming *not* removing something that would otherwise be removable.")]
    public static IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(this IServiceProvider provider,NavigableContext context) where TViewModel : ICloseableViewModel<TResult>
    {
        var view = provider.GetContextFor(typeof(TViewModel).Name, context);
        if (view != null)
        {
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
                        $"The view associated with view model {typeof(TViewModel).Name} expects result type {resultType.Name}, " +
                        $"but {typeof(TResult).Name} was requested.");
                }
            }
        }
        return view as IModalFor<TViewModel, TResult> ?? throw new InvalidOperationException($"The view associated with the view model {typeof(TViewModel).Name} is not a modal view.");
    }

    /// <summary>
    /// Synchronizes <paramref name="items"/> in place so that it contains exactly one entry per element of
    /// <paramref name="selectedItems"/>: entries for elements not yet present are added via <paramref name="add"/>,
    /// and entries with no matching element in <paramref name="selectedItems"/> are removed.
    /// </summary>
    /// <typeparam name="X">The type of the source elements to synchronize against.</typeparam>
    /// <typeparam name="Y">The type of the items in the target list.</typeparam>
    /// <param name="items">The list to update in place.</param>
    /// <param name="selectedItems">The source elements that <paramref name="items"/> should end up matching.</param>
    /// <param name="predicate">Determines whether a source element and a list item represent the same entity.</param>
    /// <param name="add">Creates a new list item from a source element that has no existing match.</param>
    public static void Update<X, Y>(this IList<Y> items, IEnumerable<X> selectedItems, Func<X, Y, bool> predicate, Func<X, Y> add)
    {
        foreach (var x in selectedItems)
            if (!items.Any(y => predicate(x, y)))
                items.Add(add(x));

        var deleted = new List<Y>();
        foreach (var item in items)
            if (!selectedItems.Any(x => predicate(x, item)))
                deleted.Add(item);

        foreach (var item in deleted)
            items.Remove(item);
    }
}