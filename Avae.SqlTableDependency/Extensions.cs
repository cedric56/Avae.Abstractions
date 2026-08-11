using Avae.DAL;
using TableDependencyCore.SqlClient;
using TableDependencyCore.SqlClient.Base.EventArgs;

namespace Avae.SqlTableDependency
{
    public static class Extensions
    {
        public static SqlTableDependencyCore<TObject> AddTableDependency<TObject>(
            this DBMonitor<TObject> monitor, string connectionString, 
            Func<TObject, long> getId,
            out Action unsuscribe)
            where TObject : class, new()
        {
            var sqlDependency = new SqlTableDependencyCore<TObject>(connectionString);
            sqlDependency.OnChanged += OnChanged;
            sqlDependency.Start();

            unsuscribe = () =>
            {
                sqlDependency.OnChanged -= OnChanged;
                sqlDependency.Stop();
            };

            return sqlDependency;

            void OnChanged(object? sender, RecordChangedEventArgs<TObject> e)
            {
                var record = new Record<TObject>(getId(e.Entity), Enum.Parse<ChangeType>(e.ChangeType.ToString()), []);
                monitor.OnChanged(record);
            }
        }
    }
}
