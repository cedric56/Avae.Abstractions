namespace Avae.DAL.Interfaces
{
    public interface ISqlMonitor
    {

    }

    public interface ISqlMonitor<T> : ISqlMonitor where T : class, new()
    {
        event EventHandler<IRecord<T>> OnChanged;
    }

    public interface IRecord<T> where T : class, new()
    {
        public long RowId { get; protected set; }
        public ChangeType ChangeType { get; protected set; }
    }

    public enum ChangeType
    {
        None,
        Delete,
        Insert,
        Update
    }
}
