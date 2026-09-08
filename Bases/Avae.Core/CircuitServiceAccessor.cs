using Microsoft.Extensions.DependencyInjection;

namespace Avae.Core;

public class CircuitServiceAccessor
{
    private static IServiceProvider? _provider;
    public static IServiceProvider Provider { get { return _provider ?? throw new InvalidOperationException("CircuitServiceAccessor.Provider is not been set."); } set { _provider = value; } }

    public TService GetRequiredService<TService>() where TService : class
    {
        return Provider.GetRequiredService<TService>();
    }
}