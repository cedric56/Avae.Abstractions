#nullable disable
using Avae.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace Avae.DAL
{
    public class LoggedConnection : DbConnection
    {
        public readonly DbConnection Inner = SimpleProvider.GetService<DbConnection>();
        private readonly ILogger _logger = SimpleProvider.GetService<ILogger>();

        protected override DbCommand CreateDbCommand()
            => new LoggedDbCommand(_logger, Inner.CreateCommand());

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

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
            => Inner.EnlistTransaction(transaction);

        //public override bool CanRaiseEvents => false;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Inner.Dispose();
            base.Dispose(disposing);
        }
    }

    public class LoggedDbCommand(ILogger Logger, DbCommand Command) : DbCommand
    {
        private readonly ILogger _logger = Logger;
        private DbCommand _command = Command;
        private bool _disposed;

        public override string CommandText
        {
            get => _command.CommandText;
            set
            {
                if (_command.Connection is SqliteConnection)
                    value = value?.Replace("SCOPE_IDENTITY", "last_insert_rowid");

                _command.CommandText = value;
            }
        }


        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }


        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }


        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }


        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }


        protected override DbParameterCollection DbParameterCollection => _command.Parameters;


        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }


        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
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
            _command?.Dispose();
            _command = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }


        public override void Cancel()
        {
            //_logger.LogDebug("Cancelling database command.");
            _command.Cancel();
        }


        public override int ExecuteNonQuery()
        {
            LogCommandBeforeExecuted();
            int result = _command.ExecuteNonQuery();
            return result;
        }


        public override object ExecuteScalar()
        {
            LogCommandBeforeExecuted();
            return _command.ExecuteScalar();
        }


        public override void Prepare()
        {
            //_logger.LogDebug("Preparing database command.");
            _command.Prepare();
        }


        protected override DbParameter CreateDbParameter() => _command.CreateParameter();


        protected override DbDataReader ExecuteDbDataReader(CommandBehavior Behavior)
        {
            LogCommandBeforeExecuted();
            return _command.ExecuteReader(Behavior);
        }


        private void LogCommandBeforeExecuted()
        {
            string request = _command.CommandText;
            foreach (IDataParameter parameter in _command.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Output ||
                  parameter.Direction == ParameterDirection.ReturnValue) continue;
                request = ReplaceWholeWord(request, $"@{parameter.ParameterName}", parameter.Value?.ToString());
            }
            Debug.WriteLine(request);
            Console.WriteLine(request);
            _logger?.LogDebug("Request: {Request}", request);
        }

        public static string ReplaceWholeWord(string s, string word, string bywhat)
        {
            char firstLetter = word[0];
            var sb = new StringBuilder();
            bool previousWasLetterOrDigit = false;
            int i = 0;
            while (i < s.Length - word.Length + 1)
            {
                bool wordFound = false;
                char c = s[i];
                if (c == firstLetter)
                    if (!previousWasLetterOrDigit)
                        if (s.Substring(i, word.Length).Equals(word))
                        {
                            wordFound = true;
                            bool wholeWordFound = true;
                            if (s.Length > i + word.Length)
                            {
                                if (char.IsLetterOrDigit(s[i + word.Length]))
                                    wholeWordFound = false;
                            }

                            if (wholeWordFound)
                                sb.Append(bywhat);
                            else
                                sb.Append(word);

                            i += word.Length;
                        }

                if (!wordFound)
                {
                    previousWasLetterOrDigit = char.IsLetterOrDigit(c);
                    sb.Append(c);
                    i++;
                }
            }

            if (s.Length - i > 0)
                sb.Append(s.AsSpan(i));

            return sb.ToString();
        }
    }
}
