namespace Avae.Abstractions;

/// <summary>
/// An interface defining how pages can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocConfiguration : IIoc
{
    void Configure(IIocContainer container);

    object? GetView(string key, params object[] @params);

    IContextFor? GetContextFor(string key, params object[] @params);

    IContextFor<TViewModel>? GetContextFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase;

    IModalFor<TViewModel>? GetModalFor<TViewModel>(params object[] @params) where TViewModel : IViewModelBase;
}
