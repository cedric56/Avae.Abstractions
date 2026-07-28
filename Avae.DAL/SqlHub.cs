using Microsoft.AspNetCore.SignalR;

namespace Avae.DAL
{
    public class SqlHub<TObject> : Hub where TObject : class, new()
    {
        public Task SendMessage(Record<TObject> record)
        {
            return Clients.All.SendAsync(Messages.DBMessage, record);
        }
    }
}
