using System;

namespace Avae.Abstractions
{
    public interface ICircuitProvider
    {
        IServiceProvider Provider { get; set; }
    }
}
