namespace Avae.DAL
{
    public interface ISqlMonitorService<T> where T : class, new()
    {
        event EventHandler<IRecord<T>> OnChanged;
    }

    public interface IRecord<T> where T : class, new()
    {
        public T Entity { get; protected set; }
        public T? EntityOldValues { get; protected set; }
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
