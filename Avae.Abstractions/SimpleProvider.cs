#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public static class SimpleProvider
    {
        static IServiceProvider provider;

        public static IServiceProvider Default => provider;

        public static void ConfigureServices(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;
        }

        public static object GetService(Type serviceType)
        {
            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            return provider.GetService(serviceType);
        }

        public static T GetService<T>()
        {
            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            return provider.GetService<T>();
        }

        public static async Task<T> GetViewModel<T>(params IParameter[] parameters) where T : class, IViewModelBase
        {
            return (T)await GetViewModel(typeof(T), parameters);
        }

        public static async Task<IViewModelBase> GetViewModel(Type viewModelType, params IParameter[] parameters)
        {
            var type = typeof(ViewModelFactory<>).MakeGenericType(viewModelType);
            if (GetService(type) is IViewModelBaseFactory factory)
            {
                var viewModel = factory.Create(viewModelType, parameters.OfType<ViewModelParameter>().ToArray());
                if (viewModel is not null)
                {
                    if (viewModel is ViewModelBase viewModelBase) await viewModelBase.OnLaunched();
                    return viewModel;
                }
                throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
            }

            if (parameters.Length > 0)
            {
                throw new InvalidOperationException("You must register a factory for view models with parameters.");
            }

            if (GetService(viewModelType) is IViewModelBase service)
            {
                if (service is ViewModelBase viewModelBase) await viewModelBase.OnLaunched();
                return service;
            }

            throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }
}