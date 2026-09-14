using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels.Tests;

public abstract class TViewModel<T> : IDisposable where T : IViewModelBase
{
    public TViewModel()
    {
        Init();
    }

    public virtual void Init()
    {
        var services = new ServiceCollection();
        Configure(services);
        _ = services.BuildServiceProvider();
    }

    public virtual void Configure(IServiceCollection services)
    {

    }

    public virtual void Dispose()
    {
        //Avae.Messenger.Messenger.Instance.Unregister(this);
    }
}
