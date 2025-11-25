namespace Avae.Abstractions;

public interface IContextFor
{
    object? DataContext { get; set; }
    static virtual string Name => throw new NotImplementedException();
}

public interface IContextFor<T> : IContextFor where T : IViewModelBase
{
    static string IContextFor.Name => typeof(T).Name;
}
