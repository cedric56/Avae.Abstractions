using Avae.Abstractions;
using Avae.DAL.Interfaces;

namespace Avae.DAL
{
    public record SqlMessage<T>(IRecord<T> Record) : Message(Messages.DBMessage, string.Empty)
        where T : class, new();

    public static class Messages
    {
        public const string DBMessage = "DBChanged";
    }
}
