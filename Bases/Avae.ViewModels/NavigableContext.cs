namespace Avae.ViewModels;

public class NavigableContext
{
    public object[] Parameters
    {
        get
        {
            var parameters = new List<object>();
            parameters.AddRange(FactoryParameters);
            parameters.AddRange(ViewParameters);
            parameters.AddRange(ViewModelParameters);
            return [.. parameters];
        }
    }

    public object[] FactoryParameters { get; set; } = [];
    public object[] ViewParameters { get; set; } = [];
    public object[] ViewModelParameters { get; set; } = [];

    public T Get<T>(int index)
    {
        var parameters = Parameters;
        if (index < 0 || index >= parameters.Length)
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Expected parameter at index {index}, but only {parameters.Length} parameter(s) were provided.");

        if (parameters[index] is not T typed)
            throw new InvalidCastException(
                $"Parameter at index {index} is of type '{parameters[index]?.GetType().Name ?? "null"}', but '{typeof(T).Name}' was expected.");

        return typed;
    }

    public static NavigableContext Create() => new();

    public NavigableContext WithFactoryParameters(params object[] parameters)
    {
        FactoryParameters = parameters;
        return this;
    }

    public NavigableContext WithViewParameters(params object[] parameters)
    {
        ViewParameters = parameters;
        return this;
    }

    public NavigableContext WithViewModelParameters(params object[] parameters)
    {
        ViewModelParameters = parameters;
        return this;
    }
}
