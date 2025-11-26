namespace Avae.Abstractions;

public delegate object ViewFactory(params object[] parameters);

/// <summary>
/// An interface defining how a page can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocContainer
{
    void Register(string key, Func<object[], object> factory);

    void Register<TContextFor>() where TContextFor : IContextFor, new();

    void Register<T>(Func<object[], object> factory);

    void Register<TContextFor>(Func<object[], TContextFor> factory) where TContextFor : IContextFor;

    void Register<TContextFor, TArg1>(Func<TArg1, TContextFor> func)
        where TContextFor : IContextFor;

    void Register<TContextFor, TArg1, TArg2>(Func<TArg1, TArg2, TContextFor> func)
        where TContextFor : IContextFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3>(Func<TArg1, TArg2, TArgs3, TContextFor> func)
        where TContextFor : IContextFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4>(Func<TArg1, TArg2, TArgs3, TArgs4, TContextFor> func)
        where TContextFor : IContextFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<TArg1, TArg2, TArgs3, TArgs4, TArgs5, TContextFor> func)
        where TContextFor : IContextFor;
}
