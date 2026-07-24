//#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public static class ServiceProviderExtensions
    {

        public static T GetViewModel<T>(this IServiceProvider provider, params IParameter[] parameters) where T : class, IViewModelBase
        {
            return (T)GetViewModel(provider,typeof(T), parameters);
        }

        public static IViewModelBase GetViewModel(this IServiceProvider provider, Type viewModelType, params IParameter[] parameters)
        {
            var type = typeof(ViewModelFactory<>).MakeGenericType(viewModelType);
            if (provider.GetService(type) is IViewModelBaseFactory factory)
            {
                var viewModel = factory.Create(viewModelType, [.. parameters.OfType<ViewModelParameter>()]);
                if (viewModel is not null)
                {
                    return viewModel;
                }
                throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider.");
            }

            if (parameters.Length > 0)
            {
                throw new InvalidOperationException("You must register a factory for view models with parameters.");
            }

            if (provider.GetService(viewModelType) is IViewModelBase service)
            {
                return service;
            }

            throw new InvalidOperationException($"Unable to create {viewModelType.Name}.  Ensure that it is registered with the service provider and it derives from {typeof(IViewModelBase).FullName}.");
        }
    }

    public static class ServiceLocator
    {
        static IServiceProvider? provider;

        public static IServiceProvider Default => provider ?? throw new InvalidOperationException("The service provider has not been configured.");

        public static void SetDefault(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;
        }

        public static T GetService<T>() where T : notnull
        {
            if (provider == null)
                throw new Exception("The service provider has not been configured. Call DefaultProvider.ConfigureServices at application startup.");
            return provider.GetRequiredService<T>();
        }
    }
}