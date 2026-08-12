using System.Data.Common;

namespace Avae.DAL
{
    public interface IDBFactory
    {
        List<IDBMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
