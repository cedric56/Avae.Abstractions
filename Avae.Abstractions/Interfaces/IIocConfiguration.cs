namespace Avae.Abstractions;

/// <summary>
/// An interface defining how pages can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocConfiguration : IIoc
{
    void Configure(IIocContainer container);

    object? GetView(string key, params object[] @params);

    IContextFor? GetContextFor(string key, params IParameter[] @params);

    IContextFor<TViewModel>? GetContextFor<TViewModel>(params IParameter[] @params) where TViewModel : IViewModelBase;

    IModalFor<TViewModel, TResult>? GetModalFor<TViewModel, TResult>(params IParameter[] @params) where TViewModel : IViewModelBase;
}
