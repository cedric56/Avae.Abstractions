using Microsoft.Extensions.DependencyInjection;

namespace Avae.Core;

public static class ServiceLocator
{
    static IServiceProvider? provider;

    public static IServiceProvider Default => provider ?? throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");

    public static void SetDefault(IServiceProvider serviceProvider)
    {
        provider = serviceProvider;
    }

    public static IServiceScope GetScoped()
    {

        if (provider == null)
            throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");
        return provider.CreateScope();
    }

    public static T GetRequiredService<T>(IServiceScope scope) where T : notnull
    {
        if (scope == null)
            throw new InvalidOperationException("Scope is null.");
        return scope.ServiceProvider.GetRequiredService<T>();
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