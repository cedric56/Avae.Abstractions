using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions;

/// <summary>0
/// An interface defining how dependencies can be configured in various frameworks such
/// as Windows, Windows Phone, Android, iOS etc.
/// </summary>
public interface IIoc
{
    /// <summary>
    /// Registers services
    /// </summary>
    /// <param name="services"></param>
    void Configure(IServiceCollection services);

    /// <summary>
    /// Defining ioc container method
    /// </summary>
    /// <param name="provider">The service provider already build</param>
    void Configure(IServiceProvider provider);
}
