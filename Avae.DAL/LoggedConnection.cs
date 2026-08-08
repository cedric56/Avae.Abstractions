using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Avae.DAL
{
    public class LoggedConnection(IServiceProvider provider) : DbConnection
    {
        public readonly DbConnection Inner = (DbConnection)provider.GetRequiredService<IDbConnection>();
        
        protected override DbCommand CreateDbCommand()
            => new LoggedDbCommand(provider.GetService<ILogger>(), Inner.CreateCommand(), provider.GetService<SqlOptions>()?.ConnectionType ?? SqlConnectionType.Unspecified);

        // Everything else MUST pass-through 1:1
        public override string ConnectionString { get => Inner.ConnectionString; set => Inner.ConnectionString = value; }
        public override string Database => Inner.Database;
        public override string DataSource => Inner.DataSource;
        public override string ServerVersion => Inner.ServerVersion;
        public override ConnectionState State => Inner.State;
        public override void ChangeDatabase(string databaseName) => Inner.ChangeDatabase(databaseName);
        public override void Close() => Inner.Close();
        public override void Open() => Inner.Open();

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            => Inner.BeginTransaction(isolationLevel);

        public override void EnlistTransaction(System.Transactions.Transaction? transaction)
            => Inner.EnlistTransaction(transaction);

        //public override bool CanRaiseEvents => false;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Inner.Dispose();
            base.Dispose(disposing);
        }
    }

    public class LoggedDbCommand(ILogger? logger, DbCommand command, SqlConnectionType connectionType) : DbCommand
    {
        private bool _disposed;

        public override string CommandText
        {
            get => command.CommandText;
            set
            {
                if (connectionType == SqlConnectionType.Sqlite)
                    value = value.Replace("SCOPE_IDENTITY", "last_insert_rowid");

                command.CommandText = value;
            }
        }


        public override int CommandTimeout
        {
            get => command.CommandTimeout;
            set => command.CommandTimeout = value;
        }


        public override CommandType CommandType
        {
            get => command.CommandType;
            set => command.CommandType = value;
        }


        public override UpdateRowSource UpdatedRowSource
        {
            get => command.UpdatedRowSource;
            set => command.UpdatedRowSource = value;
        }


        protected override DbConnection? DbConnection
        {
            get => command.Connection;
            set => command.Connection = value;
        }


        protected override DbParameterCollection DbParameterCollection => command.Parameters;


        protected override DbTransaction? DbTransaction
        {
            get => command.Transaction;
            set => command.Transaction = value;
        }


        public override bool DesignTimeVisible
        {
            get => command.DesignTimeVisible;
            set => command.DesignTimeVisible = value;
        }

        ~LoggedDbCommand() => Dispose(false);


        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            command?.Dispose();
            //command = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }


        public override void Cancel()
        {
            //_logger.LogDebug("Cancelling database command.");
            command.Cancel();
        }


        public override int ExecuteNonQuery()
        {
            LogCommandBeforeExecuted();
            int result = command.ExecuteNonQuery();
            return result;
        }


        public override object? ExecuteScalar()
        {
            LogCommandBeforeExecuted();
            return command.ExecuteScalar();
        }


        public override void Prepare()
        {
            //_logger.LogDebug("Preparing database command.");
            command.Prepare();
        }


        protected override DbParameter CreateDbParameter() => command.CreateParameter();


        protected override DbDataReader ExecuteDbDataReader(CommandBehavior Behavior)
        {
            LogCommandBeforeExecuted();
            return command.ExecuteReader(Behavior);
        }


        private void LogCommandBeforeExecuted()
        {
            string request = command.CommandText;
            foreach (IDataParameter parameter in command.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Output ||
                  parameter.Direction == ParameterDirection.ReturnValue) continue;
                request = request.ReplaceWholeWord($"@{parameter.ParameterName}", parameter.Value?.ToString() ?? string.Empty);
            }

            logger?.LogInformation("Request: {Request}", request);
        }
    }
}
