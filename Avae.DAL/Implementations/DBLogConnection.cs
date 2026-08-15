using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Avae.DAL
{
    public class DBLogConnection(IServiceProvider provider) : DbConnection
    {
        public readonly DbConnection Inner = (DbConnection)provider.GetRequiredService<IDbConnection>();
        
        protected override DbCommand CreateDbCommand()
            => new DBLogCommand(provider.GetService<ILogger>(), Inner.CreateCommand(), provider.GetService<IDBIdentity>());

        [AllowNull]
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
}
