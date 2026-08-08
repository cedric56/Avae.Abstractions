using Avae.DAL;
using Avae.DAL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Avae.SignalR
{
    public class SqlHub<TObject> : Hub where TObject : class, new()
    {
        public ISqlMonitor<TObject> monitor;

        public SqlHub(ISqlMonitor<TObject> monitor)
        {
            this.monitor = monitor;
            this.monitor.OnRecordChanged += OnRecordChanged;
        }

        public void OnRecordChanged(object? sender, Record<TObject> e)
        {
            if (Clients != null && Clients.All != null)
                _ = Task.Run(async () => await Clients.All.SendAsync(Extensions.DBMessage, e));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            monitor.OnRecordChanged -= OnRecordChanged;
        }
    }
}
