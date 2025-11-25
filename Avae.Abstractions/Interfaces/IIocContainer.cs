namespace Avae.Abstractions;

public delegate object ViewFactory(params object[] parameters);

/// <summary>
/// An interface defining how a page can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocContainer
{
    void Register(string key, Func<object[], object> factory);

    void Register<TView>() where TView : IContextFor, new();

    void Register<TViewModel>(Func<object[], object> factory);

    void Register<TView>(Func<object[], TView> factory) where TView : IContextFor;

    void Register<TView, TArg1>(Func<TArg1, TView> func)
        where TView : IContextFor;

    void Register<TView, TArg1, TArg2>(Func<TArg1, TArg2, TView> func)
        where TView : IContextFor;

    void Register<TView, TArg1, TArg2, TArgs3>(Func<TArg1, TArg2, TArgs3, TView> func)
        where TView : IContextFor;

    void Register<TView, TArg1, TArg2, TArgs3, TArgs4>(Func<TArg1, TArg2, TArgs3, TArgs4, TView> func)
        where TView : IContextFor;

    void Register<TView, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<TArg1, TArg2, TArgs3, TArgs4, TArgs5, TView> func)
        where TView : IContextFor;
}
