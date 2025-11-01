namespace Avae.Abstractions;

public interface IContextFor
{
    object? DataContext { get; set; }
}

public interface IContextFor<T> : IContextFor where T : IViewModelBase
{

}
