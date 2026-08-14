namespace Avae.DAL
{
    public static class SessionContext
    {
        public static readonly AsyncLocal<Guid?> CurrentSessionId = new();
    }

    public interface IDBMonitor
    {
        bool IsRunning { get; set; }
        Func<Task> Restart { get; set; }
    }

    public interface IDBMonitor<T> : IDBMonitor where T : class, new()
    {
        void OnChanged(Record<T> record);
        event EventHandler<Record<T>> OnRecordChanged;
    }
}
