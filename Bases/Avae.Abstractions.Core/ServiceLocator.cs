using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public static class ServiceLocator
    {
        static IServiceProvider? provider;

        public static IServiceProvider Default => provider ?? throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");

        public static void SetDefault(IServiceProvider serviceProvider)
        {
            provider = serviceProvider;
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            if (provider == null)
                throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");
            return provider.GetRequiredService<T>();
        }

        public static T? GetService<T>() where T : notnull
        {
            if (provider == null)
                throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");
            return provider.GetService<T>();
        }
    }
}