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
        private readonly Dictionary<string, ViewFactory> _factories = [];

        public IocContainer(IIocConfiguration config)
        {
            var services = new ServiceCollection();
            config.Configure(services);
            config.Configure(this);
            config.Configure(services.BuildServiceProvider());
        }

        private static T GetParameter<T>(object parameter)
        {
            return (((ViewParameter<T>)parameter).Value);
        }

        public object GetView(string key, params IParameter[] parameters)
        {
            if (_factories.TryGetValue(key, out var factory))
            {
                return factory(parameters);
            }

            throw new Exception($"No such page registered: {key}");
        }

        public void Register(string key, Func<object[], object> factory)
        {
            _factories[key] = new ViewFactory(factory);
        }

        public void Register<TView>(Func<object[], TView> factory) where TView : IContextFor
        {
            _factories[TView.Name] = new ViewFactory(args => factory.Invoke(args));
        }

        public void Register<TViewModel>(Func<object[], object> factory)
        {
            Register(typeof(TViewModel).Name, factory);
        }

        public void Register<TView>() where TView : IContextFor, new()
        {
            Register(args => new TView());
        }

        public void Register<TView, TArg1>(Func<TArg1, TView> func) where TView : IContextFor
        {
            Register(args => func(GetParameter<TArg1>(args[0])));
        }

        public void Register<TView, TArg1, TArg2>(Func<TArg1, TArg2, TView> func) where TView : IContextFor
        {
            Register(args => func(GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1])));
        }

        public void Register<TView, TArg1, TArg2, TArgs3>(Func<TArg1, TArg2, TArgs3, TView> func) where TView : IContextFor
        {
            Register(args => func(GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2])));
        }

        public void Register<TView, TArg1, TArg2, TArgs3, TArgs4>(Func<TArg1, TArg2, TArgs3, TArgs4, TView> func) where TView : IContextFor
        {
            Register(args => func(GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2]), GetParameter<TArgs4>(args[3])));
        }

        public void Register<TView, TArg1, TArg2, TArgs3, TArgs4, TArgs5>(Func<TArg1, TArg2, TArgs3, TArgs4, TArgs5, TView> func) where TView : IContextFor
        {
            Register(args => func(GetParameter<TArg1>(args[0]), GetParameter<TArg2>(args[1]), GetParameter<TArgs3>(args[2]), GetParameter<TArgs4>(args[3]), GetParameter<TArgs5>(args[4])));
        }
    }
}
