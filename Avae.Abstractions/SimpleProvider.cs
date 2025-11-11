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

        public static T GetViewModel<T>(params object[] parameters) where T : class, IViewModelBase
        {
            return GetViewModel<T>(typeof(T), parameters);
        }

        public static TBaseType GetViewModel<TBaseType>(Type viewModelType, params object[] parameters) where TBaseType : class, IViewModelBase
        {
            var type = typeof(ViewModelBaseFactory<>).MakeGenericType(viewModelType);
            var factory = GetService(type) as IViewModelBaseFactory;

            type = typeof(SingletonFactory<>).MakeGenericType(viewModelType);
            factory ??= GetService(type) as IViewModelBaseFactory;

            if (factory != null)
            {
                var viewModel = factory.Create(viewModelType, parameters);
                if (viewModel is TBaseType vm)
                {
                    Services.Add(vm);
                    return vm;
                }
                throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
            }

            if (parameters.Length > 0)
            {
                throw new InvalidOperationException("You must register a factory for view models with parameters.");
            }

            var service = GetService(viewModelType) as TBaseType;
            if (service != null)
            {
                Services.Add(service);
                return service;
            }

            throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }
}