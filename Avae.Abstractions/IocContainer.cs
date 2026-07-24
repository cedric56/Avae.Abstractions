#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    /// <summary>
    /// A simple container for registering and accessing pages in various frameworks such
    /// as Windows, Windows Phone, Android, iOS etc.
    /// </summary>
    public class IocContainer : IIocContainer
    {
        public IServiceProvider Provider { get; private set; }
        private readonly Dictionary<string, ViewFactory> _factories = [];

        public IocContainer(IIocConfiguration config)
        {
            var services = new ServiceCollection();
            config.Configure(services);
            config.Configure(this);
            config.Configure(Provider = services.BuildServiceProvider());
        }

        private static T GetParameter<T>(object parameter)
        {
            return (((ViewParameter<T>)parameter).Value);
        }

        public object GetView(string key, params object[] parameters)
        {
            if (_factories.TryGetValue(key, out var factory))
            {
                return factory(Provider, parameters);
            }

            throw new Exception($"No such page registered: {key}");
        }

        public void Register(string key, Func<IServiceProvider, object[], object> factory)
        {
            _factories[key] = new ViewFactory(factory);
        }

        public void Register<TContextFor>(Func<IServiceProvider, object[], TContextFor> factory) where TContextFor : IContextFor
        {
            _factories[TContextFor.Name] = new ViewFactory((sp, args) => factory.Invoke(sp, args));
        }

        public void Register<T>(Func<IServiceProvider, object[], object> factory)
        {
            Register(typeof(T).Name, factory);
        }

        public void Register<TContextFor>() where TContextFor : IContextFor, new()
        {
            Register((sp, args) => new TContextFor());
        }

        public void Register<TContextFor, TArg1>(Func<IServiceProvider, TArg1, TContextFor> func) where TContextFor : IContextFor
        {
            Register((sp, args) => func(sp, GetParameter<TArg1>(args[0])));
        }

        public void Register<TContextFor, TArg1, TArg2>(Func<IServiceProvider, TArg1, TArg2, TContextFor> func) where TContextFor : IContextFor
        {
            Register((sp, args) => func(sp, GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1])));
        }

        public void Register<TContextFor, TArg1, TArg2, TArgs3>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TContextFor> func) where TContextFor : IContextFor
        {
            Register((sp, args) => func(sp, GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2])));
        }

        public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TContextFor> func) where TContextFor : IContextFor
        {
            Register((sp, args) => func(sp, GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2]), GetParameter<TArgs4>(args[3])));
        }

        public void Register<TContextFor, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<IServiceProvider, TArg1, TArg2, TArgs3, TArgs4, TArgs5, TContextFor> func) where TContextFor : IContextFor
        {
            Register((sp, args) => func(sp, GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2]), GetParameter<TArgs4>(args[3]), GetParameter<TArgs5>(args[4])));
        }
    }
}
