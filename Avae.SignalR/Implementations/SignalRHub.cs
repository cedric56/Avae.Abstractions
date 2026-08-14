using Avae.DAL;
using Microsoft.AspNetCore.SignalR;

namespace Avae.SignalR
{
    public class ConnectionTracker<TObject> where TObject : class, new()
    {
        readonly HashSet<string> connections = new();
        readonly object gate = new();
        readonly IHubContext<SignalRHub<TObject>> hubContext;

        public ConnectionTracker(IHubContext<SignalRHub<TObject>> hubContext, IDBMonitor<TObject> monitor)
        {
            this.hubContext = hubContext;
            IDBFactory.Monitors.Add(monitor);
            monitor.OnRecordChanged += OnRecordChanged; // subscribed exactly ONCE, ever
        }

        public void Add(string connectionId)
        {
            lock (gate) connections.Add(connectionId);
        }

        public void Remove(string connectionId)
        {
            lock (gate) connections.Remove(connectionId);
        }

        public async void OnRecordChanged(object? sender, Record<TObject> e)
        {
            var excludedIds = e.Connections ?? [];

            List<string> notified;
            lock (gate)
            {
                notified = connections.Where(id => !excludedIds.Contains(id)).ToList();
            }

            foreach (var id in notified)
                Console.WriteLine($"Notifying customer: {id}");

            await hubContext.Clients.AllExcept(excludedIds).SendAsync(Extensions.DBMessage, e);
        }
    }

    public class SignalRHub<TObject> : Hub where TObject : class, new()
    {
        readonly ConnectionTracker<TObject> tracker;

        public SignalRHub(ConnectionTracker<TObject> tracker)
        {
            this.tracker = tracker; // no monitor subscription here anymore
        }

        public override Task OnConnectedAsync()
        {
            tracker.Add(Context.ConnectionId);
            Console.WriteLine($"Customer connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            tracker.Remove(Context.ConnectionId);
            Console.WriteLine($"Customer disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }

        public void OnRecordChanged(Record<TObject> record)
        {
            tracker.OnRecordChanged(this, record);
        }
    }
}
