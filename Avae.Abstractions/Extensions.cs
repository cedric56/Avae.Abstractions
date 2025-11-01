namespace Avae.Abstractions
{
    public static class Extensions
    {
        public static Task<TResult> ShowDialogAsync<TResult, TViewModel>(this IViewModelBase viewModelBase, params object[] values) where TViewModel : IViewModelBase
        {
            var container = SimpleProvider.GetService<IIocConfiguration>();
            var view = container?.GetModalFor<TViewModel>(values);
            return view!.ShowDialogAsync<TResult>();
        }
    }
}
