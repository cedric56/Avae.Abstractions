using Avae.DAL;
using Avae.MagicServices;
using Cysharp.Runtime.Multicast;
using MagicOnion.Server.Hubs;
using System.Collections.Concurrent;

namespace Avae.Server
{
    public class MagicOnionHub<TObject> :
    StreamingHubBase<IRecordHub<TObject>, IRecordHubReceiver<TObject>>,
    IRecordHub<TObject>, IDisposable where TObject : class, new()
    {
        public IDBMonitor<TObject> monitor;
        IGroup<IRecordHubReceiver<TObject>>? customers;

        public MagicOnionHub(IDBMonitor<TObject> monitor)
        {
            this.monitor = monitor;
            this.monitor.OnRecordChanged += MonitorChanged;
        }

        // No sessionId/receiver needed — the group call itself
        // registers the CURRENT connection (this.Context) as a member.
        public async Task AddReceiverAsync()
        {
            customers = await Group.AddAsync("customers");
        }

        // Removing the current connection from the group
        public async Task RemoveAsync()
        {
            if (customers != null)
                await customers.RemoveAsync(this.Context);
        }

        void MonitorChanged(object? sender , Record<TObject> e)
        {
            OnRecordChanged(e);
        }

        public void OnRecordChanged(Record<TObject> e)
        {
            customers?.All.OnChanged(e);
        }

        public void Dispose()
        {
            monitor.OnRecordChanged -= MonitorChanged;
        }
    }
}
