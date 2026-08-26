using Microsoft.Extensions.DependencyInjection;

namespace Avae.Core;

public class CircuitServiceAccessor
{
    public required IServiceProvider Services { get; set; }
}

public static class ServiceLocator
{
    static IServiceProvider? provider;

    public static IServiceProvider Default => provider ?? throw new InvalidOperationException("ServiceLocator.SetDefault is not been called.");

    public static void SetDefault(IServiceProvider serviceProvider)
    {
        provider = serviceProvider;
    }

    public static T GetScopedRequiredService<T>() where T : notnull
    {
        var circuit = GetRequiredService<CircuitServiceAccessor>();
        return circuit.Services.GetRequiredService<T>();
    }

    public static T? GetScopedService<T>() where T : notnull
    {
        var circuit = GetRequiredService<CircuitServiceAccessor>();
        return circuit.Services.GetService<T>();
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