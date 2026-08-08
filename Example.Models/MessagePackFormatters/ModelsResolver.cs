using Avae.DAL;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Example.Models.MessagePackFormatters
{
    public class ModelsResolver : IFormatterResolver
    {
        public static readonly ModelsResolver Instance = new();
        private static readonly PersonFormatter _personFormatter = new();

        private ModelsResolver() { }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            // ✅ This single line handles ALL types your formatter supports
            if (typeof(T) == typeof(Person) ||
                typeof(T) == typeof(DBTransactional))
            {
                return (IMessagePackFormatter<T>)_personFormatter;
            }

            return StandardResolver.Instance.GetFormatter<T>();
        }
    }
}
