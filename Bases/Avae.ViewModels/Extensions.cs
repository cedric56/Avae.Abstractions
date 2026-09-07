namespace Avae.ViewModels;

/// <summary>
/// General-purpose extension methods for resolving view models from an <see cref="IServiceProvider"/>
/// and synchronizing collections.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Resolves or creates the view model of type <typeparamref name="T"/>, using a registered
    /// <see cref="ViewModelFactory{T}"/> if one exists, or falling back to direct service resolution.
    /// </summary>
    /// <typeparam name="T">The view model type to resolve.</typeparam>
    /// <param name="provider">The service provider to resolve from.</param>
    /// <param name="context">Optional navigation context supplying constructor parameters for the view model.</param>
    /// <returns>The resolved view model instance, cast to <typeparamref name="T"/>.</returns>
    public static T GetViewModel<T>(this IServiceProvider provider, NavigableContext? context = null) where T : class, IViewModelBase
    {
        return (T)GetViewModel(provider, typeof(T), context);
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
    public static IViewModelBase GetViewModel(this IServiceProvider provider, Type viewModelType, NavigableContext? context = null)
    {
        var type = typeof(ViewModelFactory<>).MakeGenericType(viewModelType);
        if (provider.GetService(type) is IViewModelBaseFactory factory)
        {
            var viewModel = factory.Create(viewModelType, [.. context?.ViewModelParameters ?? []]);
            if (viewModel is not null)
            {
                return viewModel;
            }
            throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
        }

        if (context?.ViewModelParameters.Length > 0)
        {
            throw new InvalidOperationException("You must register a factory for view models with parameters.");
        }

        if (provider.GetService(viewModelType) is IViewModelBase service)
        {
            return service;
        }

        throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
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