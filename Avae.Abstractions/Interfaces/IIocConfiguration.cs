namespace Avae.Abstractions;

/// <summary>
/// An interface defining how pages can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocConfiguration : IIoc
{
    void Configure(IIocContainer container);

    object? GetView(string key, params object[] @params);

    IViewFor? GetContextFor(string key, NavigationContext context);

    IViewFor<TViewModel>? GetContextFor<TViewModel>(NavigationContext context) where TViewModel : IViewModelBase;

    IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(NavigationContext context) where TViewModel : ICloseableViewModel<TResult>;
}
