namespace Avae.ViewModels;

public interface IViewModelBaseFactory
{
    IViewModelBase? Create(Type viewModelType, params object[] parameters);
}

public interface IViewModelBaseFactory<T> : IViewModelBaseFactory
{

}
