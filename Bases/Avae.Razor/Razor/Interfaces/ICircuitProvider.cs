using System;

namespace Avae.Razor;

public interface ICircuitProvider
{
    IServiceProvider Provider { get; set; }
}
