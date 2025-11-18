namespace Avae.Abstractions
{
    public static class Extensions
    {
        public static async Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IViewModelBase viewModelBase, params IParameter[] parameters) where TViewModel : class, IViewModelBase
        {
            var viewModel = await SimpleProvider.GetViewModel<TViewModel>(parameters);
            var container = SimpleProvider.GetService<IIocConfiguration>();
            var view = container?.GetModalFor<TViewModel, TResult>(parameters) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
            view.DataContext = viewModel;
            return await view.ShowDialogAsync();
        }

        public static ViewModelParameter<T> ForViewModel<T>(this T obj)
        {
            return new ViewModelParameter<T>(obj);
        }

        public static ViewParameter<T> ForView<T>(this T obj)
        {
            return new ViewParameter<T>(obj);
        }

        public static FactoryParameter<T> ForFactory<T>(this T obj)
        {
            return new FactoryParameter<T>(obj);
        }
    }
}
