using System.Data.Common;

namespace Avae.DAL
{
    public interface IDbFactory
    {
        List<IDbMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
