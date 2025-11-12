namespace Avae.Abstractions
{
    public interface IViewModelBaseFactory
    {
        IViewModelBase? Create(Type viewModelType, params ViewModelParameter[] parameters);
    }

    public interface IViewModelBaseFactory<T> : IViewModelBaseFactory
    {

    }
}
