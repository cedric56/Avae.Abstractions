using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Avae.ViewModels;

public static class Extensions
{
    public static Task<TResult?> ShowModalAsync<TViewModel, TResult>(
        this IServiceProvider provider,
        NavigableContext? context = null) where TViewModel : ICloseableViewModel<TResult>
    {
        var viewModel = provider.GetViewModel<TViewModel>(context);
        var view = provider.GetModalFor<TViewModel, TResult>(context ?? new NavigableContext()) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
        view.Context = viewModel;
        return view.ShowModalAsync();
    }

    public static void RegisterWithLifetime<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
    this IServiceCollection services,
    Func<IServiceProvider, TView> func,
    ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
    ServiceLifetime viewLifetime = ServiceLifetime.Transient,
    string? key = null)
    where TView : class where TViewModel : class
    => services.RegisterPageCore<TView, TViewModel>(
        key, (sp, args) => func(sp), viewModelLifetime, viewLifetime);

    private static void RegisterFactory<T>(
        this IServiceCollection services,
        object key,
        ServiceLifetime lifetime,
        Func<IServiceProvider, object[], T> create)
        where T : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                {
                    T? cached = null;
                    var gate = new object();
                    services.AddKeyedSingleton<Func<IServiceProvider, object[], object>>(key, (sp, parameters) =>
                    {
                        if (cached is not null) return cached;
                        lock (gate) { return cached ??= create(sp, parameters); }
                    });
                    break;
                }

            case ServiceLifetime.Scoped:
                services.AddKeyedSingleton<Func<IServiceProvider, object[], object>>(key, (sp, parameters) =>
                {
                    var cache = sp.GetRequiredService<ScopedViewCache>();
                    return cache.GetOrCreate(key, () => create(sp, parameters));
                });
                break;

            case ServiceLifetime.Transient:
                services.AddKeyedSingleton<Func<IServiceProvider, object[], object>>(key, create);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }
    }

    public static void RegisterWithLifetime(
        this IServiceCollection services,
        string key,
        Func<IServiceProvider, object[], object> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        => services.RegisterFactory(key, lifetime, factory);

    private static void RegisterPageCore<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
        this IServiceCollection services,
        string? key,
        Func<IServiceProvider, object[], TView> createView,
        ServiceLifetime viewModelLifetime,
        ServiceLifetime viewLifetime)
        where TView : class where TViewModel : class
    {
        key ??= typeof(TViewModel).Name;

        ViewModelViewMap.Initialize(services).Map<TView, TViewModel>(key);
        services.RegisterViewModel<TViewModel>(viewModelLifetime);
        services.RegisterFactory(key, viewLifetime, createView);
    }

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => func(sp, (TArg1) args[0]), viewModelLifetime, viewLifetime);

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => func(sp, (TArg1) args[0], (TArg2) args[1]), viewModelLifetime, viewLifetime);

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArg3>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArg3, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => func(sp, (TArg1) args[0], (TArg2) args[1], (TArg3) args[2]),
            viewModelLifetime, viewLifetime);

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArg3, TArg4>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArg3, TArg4, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => func(sp, (TArg1) args[0], (TArg2) args[1], (TArg3) args[2], (TArg4) args[3]),
            viewModelLifetime, viewLifetime);

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1, TArg2, TArg3, TArg4, TArg5>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, TArg2, TArg3, TArg4, TArg5, TView> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => func(sp, (TArg1) args[0], (TArg2) args[1], (TArg3) args[2], (TArg4) args[3], (TArg5) args[4]),
            viewModelLifetime, viewLifetime);

    public static void RegisterWithLifetime<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
        this IServiceCollection services,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterPageCore<TView, TViewModel>(
            key, (sp, args) => ActivatorUtilities.CreateInstance<TView>(sp, args), viewModelLifetime, viewLifetime);

    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
        this IServiceCollection services, string? key = null)
        where TView : class where TViewModel : class
        => services.RegisterWithLifetime<TView, TViewModel>(
            ServiceLifetime.Singleton, ServiceLifetime.Transient, key);

    private static void RegisterViewModel<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TViewModel : class
        => services.RegisterFactory<TViewModel>(
            typeof(TViewModel), lifetime, (sp, args) => ActivatorUtilities.CreateInstance<TViewModel>(sp, args));

    public static T GetViewModel<T>(this IServiceProvider provider, NavigableContext? context = null)
        => provider.GetViewModel<T>(typeof(T), context);

    public static T GetViewModel<T>(this IServiceProvider provider, object key, NavigableContext? context = null)
        => (T)GetViewModel(provider, key, context);

    public static object GetViewModel(this IServiceProvider provider, object key, NavigableContext? context = null)
    {
        var factory = provider.GetKeyedService<Func<IServiceProvider, object[], object>>(key)
            ?? throw new InvalidOperationException($"Unable to create {key}. Ensure that it is registered with the service provider.");

        return factory(provider, [.. context?.ViewModelParameters ?? []])
            ?? throw new InvalidOperationException($"Factory for {key} returned null.");
    }

    public static object GetView(this IServiceProvider provider, string key, object[] context)
    {
        var factory = provider.GetKeyedService<Func<IServiceProvider, object[], object>>(key)
            ?? throw new Exception($"No such view registered: {key}");

        return factory(provider, [.. context ?? []])
            ?? throw new InvalidOperationException($"Unable to create view for {key}. Ensure that it is registered with the service provider.");
    }

    public static IViewFor? GetContextFor(this IServiceProvider provider, object key, NavigableContext? context = null)
    {
        context ??= new NavigableContext();
        var resolvedKey = context.Key ?? key;

        if (resolvedKey is not string stringKey)
            throw new InvalidOperationException($"GetContextFor requires a string key, got {resolvedKey?.GetType().Name ?? "null"}.");

        var view = provider.GetView(stringKey, [.. context.ViewParameters ?? []]);
        return view as IViewFor
            ?? throw new InvalidOperationException($"View must implement {nameof(IViewFor)}");
    }

    public static IViewFor<TViewModel>? GetContextFor<TViewModel>(this IServiceProvider provider, NavigableContext context)
        => provider.GetContextFor(typeof(TViewModel).Name, context) as IViewFor<TViewModel>;

    [UnconditionalSuppressMessage("Trimming", "IL2075",
        Justification = "View types are always registered via explicit compile-time factories " +
                         "in RegisterPageCore. The concrete type and its constructor are referenced " +
                         "directly at the registration call site, so the trimmer already roots that " +
                         "type — including the interfaces it implements.")]
    public static IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(
        this IServiceProvider provider, NavigableContext context)
        where TViewModel : ICloseableViewModel<TResult>
    {
        var view = provider.GetContextFor(typeof(TViewModel).Name, context);
        if (view != null)
        {
            var modalInterface = view.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

            if (modalInterface != null)
            {
                var resultType = modalInterface.GetGenericArguments()[1];
                if (resultType != typeof(TResult))
                    throw new InvalidOperationException(
                        $"The view associated with view model {typeof(TViewModel).Name} expects result type " +
                        $"{resultType.Name}, but {typeof(TResult).Name} was requested.");
            }
        }

        return view as IModalFor<TViewModel, TResult>
            ?? throw new InvalidOperationException(
                $"The view associated with the view model {typeof(TViewModel).Name} is not a modal view.");
    }

    public static void Update<X, Y>(this IList<Y> items, IEnumerable<X> selectedItems, Func<X, Y, bool> predicate, Func<X, Y> add)
    {
        foreach (var x in selectedItems)
            if (!items.Any(y => predicate(x, y)))
                items.Add(add(x));

        var deleted = items.Where(item => !selectedItems.Any(x => predicate(x, item))).ToList();
        foreach (var item in deleted)
            items.Remove(item);
    }
}