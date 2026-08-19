using Avae.Core;

namespace Avae.DAL;

public class DBBase
{
    private static readonly object _lock = new();
    private static IDBLayer? _instance;
    public static IDBLayer Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= ServiceLocator.GetRequiredService<IDBLayer>();
                }
            }
            return _instance;
        }
    }
}
