namespace Avae.ViewModels;

public static class Extensions
{
    public static T GetViewModel<T>(this IServiceProvider provider, NavigationContext? context = null) where T : class, IViewModelBase
    {
        return (T)GetViewModel(provider, typeof(T), context);
    }

    public static IViewModelBase GetViewModel(this IServiceProvider provider, Type viewModelType, NavigationContext? context = null)
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
