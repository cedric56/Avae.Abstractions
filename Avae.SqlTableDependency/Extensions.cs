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
            this SqlMonitor<TObject> monitor, string connectionString, out Action dispose)
            where TObject : class, new()
        {
            var sqlDependency = new SqlTableDependencyCore<TObject>(connectionString);
            sqlDependency.OnChanged += OnChanged;
            sqlDependency.Start();

            dispose = () =>
            {
                sqlDependency.OnChanged -= OnChanged;
                sqlDependency.Stop();
            };

            return sqlDependency;

            void OnChanged(object? sender, RecordChangedEventArgs<TObject> e)
            {
                if (e.Entity is IModelBase model)
                {
                    var record = new Record<TObject>(model.Id, Enum.Parse<ChangeType>(e.ChangeType.ToString()));
                    monitor.Changed(record);
                }
            }
        }
    }
}
