using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Avae.DAL;

public sealed class DBTransactionalResolver : IFormatterResolver
{
    List<IMessagePackFormatter> _formatters = [new DBTransactionalFormatter()];

    public void Register<T>(IDBTransactionalFormatter formatter) where T : DBTransactional?
    {
        _formatters.Add(formatter);
        DBTransactionalFormatter.Register<T>(formatter);
    }

    private static DBTransactionalResolver GetInstance()
    {
        var resolver = new DBTransactionalResolver();
        MessagePackSerializer.DefaultOptions.WithResolver(CompositeResolver.Create(
            resolver
        ));
        return resolver;
    }

    public static DBTransactionalResolver Instance = GetInstance();

    private DBTransactionalResolver()
    {

    }

    public IMessagePackFormatter? GetFormatter(Type? type)
    {
        if (type == null)
            return null;

        var generic =  typeof(IMessagePackFormatter<>).MakeGenericType(type);
        return _formatters.FirstOrDefault(f => f.GetType() == generic);
    }

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        var formatter = _formatters.OfType<IMessagePackFormatter<T>>().FirstOrDefault();
        return formatter ?? BuiltinResolver.Instance.GetFormatter<T>() ?? StandardResolver.Instance.GetFormatter<T>() ?? throw new NotImplementedException();
    }
}

public class DBTransactionalSerializerOptions(DBTransactionalResolver resolver) : MessagePackSerializerOptions(resolver)
{

}
