using Avae.DAL;
using Avae.Server;
using Example.Models;

namespace Example.Server
{
    public class OnionHub : MagicOnionHub<Person>
    {
        public OnionHub(IDBMonitor<Person> monitor) : base(monitor)
        {

        }
    }
}
