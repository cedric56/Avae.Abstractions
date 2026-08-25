using MessagePack;
using MessagePack.Formatters;
using System.Text;

namespace Avae.DAL;

public interface IDBTransactionalFormatter : IMessagePackFormatter
{
    void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options);

    object? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options);
}

//public class DBTransactionalFormatter<T>(IMessagePackFormatter<T?> formatter) : 
//    IDBTransactionnalFormatter where T : DBTransactional?
//{
//    public void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options)
//    {
//        formatter.Serialize(ref writer, value as T, options);
//    }

//    public object? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
//    {
//        return formatter.Deserialize(ref reader, options);
//    }
//}

internal class DBTransactionalFormatter : IMessagePackFormatter<DBTransactional?>
{
    private static Dictionary<Type, IDBTransactionalFormatter> _formatters = new();
    public static void Register<T>(IDBTransactionalFormatter formatter) where T : DBTransactional?
    {
        _formatters.Add(typeof(T), formatter);
    }

    public void Serialize(ref MessagePackWriter writer, DBTransactional? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }

        var type = value.GetType();
        var typeName = type.AssemblyQualifiedName ?? type.FullName;

        // Write as array: [TypeName, Data]
        writer.WriteArrayHeader(2);
        writer.WriteString(Encoding.UTF8.GetBytes(typeName ?? string.Empty));

        if (_formatters.TryGetValue(type, out var formatter))
            formatter.Serialize(ref writer, value, options);
        else
            writer.WriteNil();
    }

    public DBTransactional? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
            return null;

        var count = reader.ReadArrayHeader();
        if (count != 2)
            throw new MessagePackSerializationException($"Invalid format: expected 2 fields, got {count}");

        var typeName = reader.ReadString();
        if (string.IsNullOrEmpty(typeName))
            throw new MessagePackSerializationException("Type name is null or empty");

        var targetType = Type.GetType(typeName) ?? throw new MessagePackSerializationException($"Could not resolve type: {typeName}");
        if (_formatters.TryGetValue(targetType, out var formatter))
            return formatter.Deserialize(ref reader, options) as DBTransactional;

        throw new NotImplementedException($"Must Register type : {targetType} on {nameof(DBTransactionalResolver)}");
    }
}
