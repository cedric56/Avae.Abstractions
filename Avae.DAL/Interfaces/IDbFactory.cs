using System.Data.Common;

namespace Avae.DAL
{
    public interface IDbFactory
    {
        List<ISqlMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
