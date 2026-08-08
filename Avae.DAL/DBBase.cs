using Avae.Abstractions;
using Avae.DAL.Interfaces;

namespace Avae.DAL
{
    public class DBBase
    {
        private static readonly object _lock = new();
        private static IDataAccessLayer? _instance;
        public static IDataAccessLayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = ServiceLocator.GetRequiredService<IDataAccessLayer>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
