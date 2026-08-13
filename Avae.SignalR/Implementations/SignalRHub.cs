using Avae.DAL;
using Microsoft.AspNetCore.SignalR;

namespace Avae.SignalR
{
    public class SignalRHub<TObject> : Hub where TObject : class, new()
    {
        public IDBMonitor<TObject> monitor;

        public SignalRHub(IDBMonitor<TObject> monitor)
        {
            IDBFactory.Monitors.Add(monitor);
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
