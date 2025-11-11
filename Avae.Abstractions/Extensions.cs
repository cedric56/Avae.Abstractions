namespace Avae.Abstractions
{
    public static class Extensions
    {
        public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IViewModelBase viewModelBase, params object[] viewParameters) where TViewModel : class, IViewModelBase
        {
            var vm = SimpleProvider.GetViewModel<TViewModel>();
            return ShowDialogAsync<TViewModel, TResult>(vm, viewParameters);
        }

        public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IViewModelBase viewModelBase, object[] viewModelsParameters, params object[] viewParameters) where TViewModel : class, IViewModelBase
        {
            var vm = SimpleProvider.GetViewModel<TViewModel>(viewModelsParameters);
            return ShowDialogAsync<TViewModel, TResult>(vm, viewParameters);
        }

        public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(TViewModel viewModel, params object[] viewParameters) where TViewModel : IViewModelBase
        {
            var container = SimpleProvider.GetService<IIocConfiguration>();
            var view = container?.GetModalFor<TViewModel, TResult>(viewParameters) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
            view.DataContext = viewModel;
            return view.ShowDialogAsync();
        }
    }
}
