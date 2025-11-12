#nullable disable
namespace Avae.Abstractions
{
    public interface IParameter
    {
        object Value { get; }
    }

    public class ViewModelParameter(object obj) : IParameter
    {
        public object Value => obj;
    }

    public class ViewModelParameter<T>(T obj) :
        ViewModelParameter(obj)
    {
        public new T Value => obj;
    }

    public class ViewParameter(object obj) : IParameter
    {
        public object Value => obj;
    }

    public class ViewParameter<T>(T obj) :
        ViewParameter(obj)
    {
        public new T Value => obj;
    }
    public class FactoryParameter(object obj) : IParameter
    {
        public object Value => obj;
    }
    public class FactoryParameter<T>(T obj) :
        FactoryParameter(obj)
    {
        public new T Value => obj;
    }
}
