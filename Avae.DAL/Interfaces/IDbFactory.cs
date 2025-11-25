using System.Data.Common;

namespace Avae.DAL.Interfaces
{
    public interface IDbFactory
    {
        List<ISqlMonitor> Monitors { get; }
        DbConnection? CreateConnection();
    }
}
