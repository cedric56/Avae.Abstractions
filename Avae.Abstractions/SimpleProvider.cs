#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public static class SimpleProvider
    {
        static IServiceProvider provider;
        public static List<object> Services = new List<object>();

        public static IServiceProvider Default => provider;

        public static void ConfigureServices(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;
        }

        public static object GetService(Type serviceType)
        {
            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            var service = provider.GetService(serviceType);
            Services.Add(service);
            return service;
        }

        public static T GetService<T>()
        {
            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            var service = provider.GetService<T>();
            Services.Add(service);
            return service;
        }

        public static T GetViewModel<T>(params object[] parameters) where T : IViewModelBase
        {
            object service = null;

            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            var factory = provider.GetService<ViewModelBaseFactory<T>>();
            factory ??= provider.GetService<SingletonFactory<T>>();
            if (factory != null)
            {
                service = factory?.Create(typeof(T), parameters);
                Services.Add(service);
                return (T)service;
            }
            if (parameters.Length > 0)
            {
                throw new Exception("You must register a factory for view models with parameters.");
            }
            service = provider.GetService<T>();
            Services.Add(service);
            return (T)service;
        }
    }
}