namespace Avae.DAL;

public interface IDBMonitor
{
    [Obsolete]
    bool IsRunning { get; set; }

    [Obsolete]
    Func<Task> Restart { get; set; }
}

public interface IDBMonitor<T> : IDBMonitor where T : class, new()
{
    void OnChanged(Record<T> record);
    event EventHandler<Record<T>> OnRecordChanged;
}
