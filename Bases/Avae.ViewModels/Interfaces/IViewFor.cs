namespace Avae.ViewModels;

public interface IViewFor
{
    object? Context { get; set; }

    static virtual string Name => throw new NotImplementedException();
}

public interface IViewFor<T> : IViewFor where T : IViewModelBase
{
    static string IViewFor.Name => typeof(T).Name;
}
