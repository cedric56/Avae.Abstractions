namespace Avae.ViewModels;

/// <summary>
/// An interface defining how pages can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocConfiguration : IIoc
{
    void Configure(IIocContainer container);

    object? GetView(string key, params object[] @params);

    IViewFor? GetContextFor(string key, NavigableContext context);

    IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigableContext context) where TViewModel : IViewModelBase;

    IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigableContext context) where TViewModel : ICloseableViewModel<TResult>;
}
