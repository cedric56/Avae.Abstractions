using Avae.Abstractions;
using SqlTableDependency.Extensions;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;

namespace Avae.DAL
{
    public class SqlDependencyExService<T>(string connectionString, string tableName = null, string schemaName = null, IModelToTableMapper<T> mapper = null, IUpdateOfModel<T> updateOf = null, ITableDependencyFilter filter = null, DmlTriggerType notifyOn = DmlTriggerType.All, bool executeUserPermissionCheck = true, bool includeOldValues = false) : IDisposable, ISqlDependencyExService<T> where T : class, new()
    {
        private readonly SqlTableDependencyEx<T> _dependencyEx = new(connectionString,
                tableName, schemaName, mapper, updateOf, filter, notifyOn, executeUserPermissionCheck, includeOldValues);

        TableDependency.SqlClient.Base.Delegates.ErrorEventHandler? _OnError = null;

        ChangedEventHandler<T>? _OnChanged = null;

        StatusEventHandler? _OnStatusChanged = null;

        public event ChangedEventHandler<T> OnOriginalChanged
        {
            add { _dependencyEx.OnChanged += value; }
            remove { _dependencyEx.OnChanged -= value; }
        }
        public event EventHandler<IError> OnError
        {
            add
            {
                _dependencyEx.OnError += _OnError = (sender, e) =>
                {
                    var error = new SqlDependencyError(e.Message, e.Error);
                    value.Invoke(sender, error);
                };
            }
            remove { _dependencyEx.OnError -= _OnError; }
        }
        public event EventHandler<IRecord<T>> OnChanged
        {
            add
            {
                _dependencyEx.OnChanged += _OnChanged = (sender, e) =>
                {
                    Enum.TryParse<ChangeType>(e.ChangeType.ToString(), out var result);
                    var record = new Record<T>(e.Entity, result, e.EntityOldValues);
                    value.Invoke(sender, record);
                };
            }
            remove { _dependencyEx.OnChanged -= _OnChanged; }
        }

        public event EventHandler<TableDependencyStatus> OnStatusChanged
        {
            add
            {
                _dependencyEx.OnStatusChanged += _OnStatusChanged = (sender, e) =>
                {
                    Enum.TryParse<TableDependencyStatus>(e.Status.ToString(), out var result);
                    value.Invoke(sender, result);
                };
            }
            remove { _dependencyEx.OnStatusChanged -= _OnStatusChanged; }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _dependencyEx.Start();
        }

        public void Stop()
        {
            _dependencyEx.Stop();
            _dependencyEx.OnStatusChanged -= _OnStatusChanged;
            _dependencyEx.OnChanged -= _OnChanged;
            _dependencyEx.OnError -= _OnError;
        }
    }

    public class SqlDependencyError(string message, Exception exception) : IError
    {
        string IError.Message { get => message; set => message = value; }
        Exception IError.Error { get => exception; set => exception = value; }
    }
}
