using Avae.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Avae.ViewModels;

/// <summary>
/// A simple container for registering and accessing pages in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public class IocContainer : IIocContainer
{
    private IServiceProvider? _provider;
    public IServiceProvider Provider { get { return _provider ??= ServiceLocator.Default; } private set { _provider = value; } }
    private readonly ConcurrentDictionary<string, ViewFactory> _factories = [];

    public IocContainer(IIocConfiguration config, bool buildServiceProvider = true)
    {
        var services = new ServiceCollection();
        config.Configure(services);
        config.Configure(this);
        if (buildServiceProvider)
        {
            config.Configure(_provider = services.BuildServiceProvider());
        }
    }

    public void SetProvider(IServiceProvider provider)
    {
        _provider = provider;
    }

    public object GetView(string key, object[] context)
    {
        if (_factories.TryGetValue(key, out var factory))
        {
            return factory(Provider, context);
        }

        throw new Exception($"No such page registered: {key}");
    }

    public IModalFor<T, TResult> GetModal<T, TResult>(NavigableContext context) where T : ICloseableViewModel<TResult>
    {
        var view = GetView(typeof(T).Name, [context]);

        // Now verify the TResult type matches
        Type viewType = view.GetType();
        Type[] interfaces = viewType.GetInterfaces();

        // Find the IModalFor<,> interface implementation
        var modalInterface = interfaces.FirstOrDefault(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModalFor<,>));

        if (modalInterface != null)
        {
            Type[] genericArgs = modalInterface.GetGenericArguments();
            Type viewModelType = genericArgs[0];
            Type resultType = genericArgs[1];

            // Check if the TResult matches
            if (resultType != typeof(TResult))
            {
                throw new InvalidOperationException(
                    $"The view associated with view model {typeof(T).Name} expects result type {resultType.Name}, " +
                    $"but {typeof(TResult).Name} was requested.");
            }
        }

        return view as IModalFor<T, TResult> ?? throw new InvalidOperationException($"The view associated with the view model {typeof(T).Name} is not a modal view.");
    }

    public void Register(string key, Func<IServiceProvider, object[], object> factory)
    {
        _factories[key] = new ViewFactory(factory);
    }

    public void Register<TContextFor>(Func<IServiceProvider, NavigableContext, TContextFor> factory) where TContextFor : IViewFor
    {
        _factories[TContextFor.Name] = new ViewFactory((sp, args) => factory.Invoke(sp, (NavigableContext)args[0]));
    }

    public void Register<T>(Func<IServiceProvider, NavigableContext, object> factory)
    {
        _factories[typeof(T).Name] = new ViewFactory((sp, args) => factory.Invoke(sp, (NavigableContext)args[0]));
    }

    public void Register<TContextFor>() where TContextFor : IViewFor, new()
    {
        Register((sp, args) => new TContextFor());
    }

    public void Register<TContextFor, TArg1>(Func<IServiceProvider, TArg1, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, (TArg1)args.Parameters[0]));
    }

    public void Register<TContextFor, TArg1, TArg2>(Func<IServiceProvider, TArg1, TArg2, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, (TArg1)args.Parameters[0], (TArg2)args.Parameters[1]));
    }

    public void Register<TContextFor, TArg1, TArg2, TArgs3>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, (TArg1)args.Parameters[0], (TArg2)args.Parameters[1], (TArgs3)args.Parameters[2]));
    }

    public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, (TArg1)args.Parameters[0], (TArg2)args.Parameters[1], (TArgs3)args.Parameters[2], (TArgs4)args.Parameters[3]));
    }

    public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TArgs5, TContextFor> func) where TContextFor : IViewFor
    {
        Register((sp, args) => func(sp, (TArg1)args.Parameters[0], (TArg2)args.Parameters[1], (TArgs3)args.Parameters[2], (TArgs4)args.Parameters[3], (TArgs5)args.Parameters[4]));
    }
}
