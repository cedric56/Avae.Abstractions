namespace Avae.Abstractions;

public class NavigationContext
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
}
