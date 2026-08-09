using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Avae.DAL
{
    public sealed class UnionResolver : IFormatterResolver
    {
        List<IMessagePackFormatter> _formatters = [new DBTransactionalFormatter()];

        public void Register<T>(IDBTransactionalFormatter formatter) where T : DBTransactional?
        {
            _formatters.Add(formatter);
            DBTransactionalFormatter.Register<T>(formatter);
        }

        private static UnionResolver GetInstance()
        {
            var resolver = new UnionResolver();
            MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions.WithResolver(CompositeResolver.Create(

                StandardResolver.Instance,       // For primitive types
                BuiltinResolver.Instance,         // For built-in types
                resolver
            ));
            return resolver;
        }

        public static UnionResolver Instance = GetInstance();

        private UnionResolver()
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
            return formatter ?? StandardResolver.Instance.GetFormatter<T>() ?? throw new NotImplementedException();
        }
    }

    public class UnionMessagePackSerializerOptions(UnionResolver resolver) : MessagePackSerializerOptions(resolver)
    {

    }
}
