//#nullable disable
using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
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