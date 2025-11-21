namespace Avae.DAL
{
    public interface ISqlDependencyExService<T> : ISqlMonitorService<T> where T : class, new()
    {
        void Start();

        void Stop();

        event EventHandler<IError> OnError;

        event EventHandler<TableDependencyStatus> OnStatusChanged;

    }
    
    public enum TableDependencyStatus
    {
        None,
        Starting,
        Started,
        WaitingForNotification,
        StopDueToCancellation,
        StopDueToError
    }    

    public interface IError
    {
        public string Message { get; protected set; }

        public Exception Error { get; protected set; }
    }
}
