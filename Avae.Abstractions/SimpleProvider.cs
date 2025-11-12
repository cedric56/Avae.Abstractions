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

        public static T GetViewModel<T>(params IParameter[] parameters) where T : class, IViewModelBase
        {
            return (T)GetViewModel(typeof(T), parameters);
        }

        public static IViewModelBase GetViewModel(Type viewModelType, params IParameter[] parameters)
        {
            var type = typeof(ViewModelFactory<>).MakeGenericType(viewModelType);
            var factory = GetService(type) as IViewModelBaseFactory;
            if (factory != null)
            {
                var viewModel = factory.Create(viewModelType, parameters.OfType<ViewModelParameter>().ToArray());
                if (viewModel is not null)
                {
                    Services.Add(viewModel);
                    return viewModel;
                }
                throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
            }

            if (parameters.Length > 0)
            {
                throw new InvalidOperationException("You must register a factory for view models with parameters.");
            }

            var service = GetService(viewModelType) as IViewModelBase;
            if (service != null)
            {
                Services.Add(service);
                return service;
            }

            throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }
}