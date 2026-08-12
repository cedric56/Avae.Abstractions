namespace Avae.DAL
{
    public interface IDBMonitor
    {

    }

    public interface ISqlMonitor<T> : IDBMonitor where T : class, new()
    {
        event EventHandler<Record<T>> OnRecordChanged;
    }
}
