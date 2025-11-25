using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public abstract class ViewModelFactory : IViewModelBaseFactory
    {
        public abstract IViewModelBase? Create(Type viewModelType, params ViewModelParameter[] parameters);
    }

    public class ViewModelFactory<T>(IServiceProvider provider) : ViewModelFactory, IViewModelBaseFactory<T> where T : IViewModelBase
    {
        private T? viewModel = default;
        protected readonly IServiceProvider _provider = provider;

        public override IViewModelBase? Create(Type viewModelType, params ViewModelParameter[] parameters)
            => (IViewModelBase?)(viewModel ??= (T)ActivatorUtilities.CreateInstance(_provider, viewModelType, [.. parameters.Select(p => p.Value)]));
    }
}
