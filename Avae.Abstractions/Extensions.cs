namespace Avae.Abstractions
{
    public static class Extensions
    {
        public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IViewModelBase viewModelBase, params object[] values) where TViewModel : IViewModelBase
        {
            var container = SimpleProvider.GetService<IIocConfiguration>();
            var view = container?.GetModalFor<TViewModel, TResult>(values) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
            return view.ShowDialogAsync();
        }
    }
}
