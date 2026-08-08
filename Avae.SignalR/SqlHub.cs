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

        private void OnRecordChanged(object? sender, IRecord<TObject> e)
        {
            SendMessage(e);
        }

        public void SendMessage(object record)
        {
            if (Clients != null && Clients.All != null)
                _ = Task.Run(async () => await Clients.All.SendAsync(Extensions.DBMessage, record));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            monitor.OnRecordChanged -= OnRecordChanged;
        }
    }
}
