using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels;

public abstract class ViewModelFactory : IViewModelBaseFactory
{
    public abstract IViewModelBase? Create(Type viewModelType, params object[] parameters);
}

public class ViewModelFactory<T>(IServiceProvider provider) : ViewModelFactory, IViewModelBaseFactory<T> where T : IViewModelBase
{
    private T? viewModel = default;

    public override IViewModelBase? Create(Type viewModelType, params object[] parameters)
        => (IViewModelBase?)(viewModel ??= (T)ActivatorUtilities.CreateInstance(provider, viewModelType, [.. parameters]));
}
