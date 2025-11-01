namespace Avae.Abstractions;

public delegate object ViewFactory(params object[] parameters);

/// <summary>
/// An interface defining how a page can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocContainer
{
    /// <summary>
    /// Registers a view based on viewmodel
    /// </summary>
    /// <typeparam name="TView">Type of the view</typeparam>
    /// <typeparam name="TViewModel">Type of the viewModel</typeparam>
    void Register<TViewModel, TView>();

    /// <summary>
    /// Registers a iviewfor
    /// </summary>
    /// <typeparam name="TView">Type of the view</typeparam>
    void Register<TViewFor>() where TViewFor : IContextFor;

    /// <summary>
    /// Registers a view based on a key
    /// </summary>
    /// <param name="key">Key to registered</param>
    /// <param name="type">Type to registered</param>
    void Register(string key, Type type);

    /// <summary>
    /// Registers the View factory for the specified viewmodel type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="factory">The view factory.</param>
    void Register<T>(ViewFactory? factory = null);

    /// <summary>
    /// Registers the View factory for the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="factory">The view factory.</param>
    void RegisterFactory(string key, ViewFactory factory);

    void Register(string key, Func<object[], object> action);

}
