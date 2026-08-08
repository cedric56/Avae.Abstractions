using System.Data.Common;

namespace Avae.DAL.Interfaces
{
    public interface IDBFactory
    {
        List<ISqlMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
