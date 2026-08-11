namespace Avae.Abstractions;

public delegate object ViewFactory(IServiceProvider serviceProvider, params object[] context);

/// <summary>
/// An interface defining how a page can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIocContainer
{
    void Register(string key, Func<IServiceProvider, object[], object> factory);

    void Register<TContextFor>() where TContextFor : IViewFor, new();

    void Register<T>(Func<IServiceProvider, NavigationContext, object> factory);

    void Register<TContextFor>(Func<IServiceProvider, NavigationContext, TContextFor> factory) where TContextFor : IViewFor;

    void Register<TContextFor, TArg1>(Func<IServiceProvider, TArg1, TContextFor> func)
        where TContextFor : IViewFor;

    void Register<TContextFor, TArg1, TArg2>(Func<IServiceProvider, TArg1, TArg2, TContextFor> func)
        where TContextFor : IViewFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TContextFor> func)
        where TContextFor : IViewFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TContextFor> func)
        where TContextFor : IViewFor;

    void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TArgs5, TContextFor> func)
        where TContextFor : IViewFor;
}
