using Avae.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels.Tests;

public abstract class TViewModel<T> : IIoc, IDisposable where T : IViewModelBase
{
    public TViewModel()
    {
        Init();
    }

    public virtual void Init()
    {
        var services = new ServiceCollection();
        Configure(services);
        Configure(services.BuildServiceProvider());
    }

    public virtual void Configure(IServiceCollection services)
    {

    }

    public virtual void Configure(IServiceProvider serviceProvider)
    {
        ServiceLocator.SetDefault(serviceProvider);
    }

    public virtual void Dispose()
    {
        //Messenger.Instance.Unregister(this);
    }
}
