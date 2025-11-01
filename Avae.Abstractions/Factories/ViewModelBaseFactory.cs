using Microsoft.Extensions.DependencyInjection;

namespace Avae.Abstractions
{
    public class ViewModelBaseFactory : IViewModelBaseFactory
    {
        public IViewModelBase? Create(Type viewModelType, params object[] parameters)
            => throw new NotImplementedException("Use the generic version of this factory.");
    }

    public class SingletonFactory<T> : ViewModelBaseFactory<T> where T : IViewModelBase
    {
        private T? viewModel = default;
        public SingletonFactory(IServiceProvider provider)
            : base(provider)
        {

        }

        public override IViewModelBase? Create(Type viewModelType, params object[] parameters)
            => (IViewModelBase?)(viewModel ?? (viewModel = (T)ActivatorUtilities.CreateInstance(_provider, viewModelType, parameters)));
    }

    public class ViewModelBaseFactory<T> : ViewModelBaseFactory, IViewModelBaseFactory<T> where T : IViewModelBase
    {
        protected readonly IServiceProvider _provider;
        public ViewModelBaseFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public new virtual IViewModelBase? Create(Type viewModelType, params object[] parameters)
            => (IViewModelBase?)ActivatorUtilities.CreateInstance(_provider, viewModelType, parameters);
    }
}
