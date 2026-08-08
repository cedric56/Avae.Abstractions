using System.Data.Common;

namespace Avae.DAL.Interfaces
{
    public interface IDBFactory
    {
        List<IDBMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
