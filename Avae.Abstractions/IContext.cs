namespace Avae.Abstractions;

public interface IContext
{
    object[] ViewParameters { get; set; }
}

public interface IContext<T> : IContext where T : IViewModelBase
{
}
