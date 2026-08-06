using System.Text;

namespace Avae.Abstractions
{
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

        

        public static void Update<X, Y>(this IList<Y> items, IList<X> selectedItems, Func<X, Y, bool> predicate, Func<X, Y> add)
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

        public static string ToFullBlownString(this Exception e, int level = int.MaxValue)
        {
            var sb = new StringBuilder();
            var exception = e;
            var counter = 1;
            while (exception != null && counter <= level)
            {
                //var stackFrame = (new StackTrace(exception, true)).GetFrame(0);
                //var message = string.Format("At line {0} column {1} in {2}: {3} {4}{3}{5}  ",
                //   stackFrame.GetFileLineNumber(), stackFrame.GetFileColumnNumber(),
                //   stackFrame.GetMethod(), Environment.NewLine, stackFrame.GetFileName(),
                //   exception.Message);

                sb.AppendLine($"{counter}-> Level: {counter}");
                sb.AppendLine($"{counter}-> Message: {exception.Message}");
                sb.AppendLine($"{counter}-> Source: {exception.Source}");
                sb.AppendLine($"{counter}-> Target Site: {exception.TargetSite}");
                sb.AppendLine($"{counter}-> Stack Trace: {exception.StackTrace}");
                //sb.AppendLine($"{counter}-> Formatted: {message}");

                exception = exception.InnerException;
                counter++;
            }

            return sb.ToString();
        }
    }
}
