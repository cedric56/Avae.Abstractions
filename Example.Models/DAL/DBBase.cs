using Avae.Abstractions;

namespace Example.Models
{
    public class DBBase
    {
        private static object _lock = new object();
        private static IDbLayer? _instance;
        public static IDbLayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = SimpleProvider.GetService<IDbLayer>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
