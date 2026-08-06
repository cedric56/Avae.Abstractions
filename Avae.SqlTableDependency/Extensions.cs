using Avae.Abstractions;
using Avae.DAL;
using Avae.DAL.Interfaces;
using TableDependencyCore.SqlClient;
using TableDependencyCore.SqlClient.Base.EventArgs;

namespace Avae.SqlTableDependency
{
    public static class Extensions
    {
        public static SqlTableDependencyCore<TObject> AddTableDependency<TObject>(
            this SqlMonitor<TObject> monitor, string connectionString)
            where TObject : class, new()
        {
            var sqlDependency = new SqlTableDependencyCore<TObject>(connectionString);
            sqlDependency.OnChanged += OnChanged;
            sqlDependency.Start();

            return sqlDependency;

            void OnChanged(object? sender, RecordChangedEventArgs<TObject> e)
            {
                if (e.Entity is IModelBase model)
                {
                    var record = new Record<TObject>(model.Id, Enum.Parse<ChangeType>(e.ChangeType.ToString()));
                    monitor.Changed(record);
                }
            }

            //TODO Cleanup
            //public void Dispose()
            //{
            //    if (sqlDependency is not null)
            //    {
            //        sqlDependency.OnChanged -= SqlDependencyExService_OnChanged;
            //        sqlDependency.Stop();
            //        sqlDependency.Dispose();
            //    }

            //    GC.SuppressFinalize(this);
            //}
        }
    }
}
