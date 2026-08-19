using Microsoft.AspNetCore.SignalR;

namespace Avae.DAL.SignalR;

public class ConnectionTracker<TObject> : IDisposable where TObject : class, new()
{
    readonly HashSet<string> connections = new();
    readonly object gate = new();
    readonly IHubContext<SignalRHub<TObject>> hubContext;

    readonly IDBMonitor<TObject> monitor;

    public ConnectionTracker(IHubContext<SignalRHub<TObject>> hubContext, IDBMonitor<TObject> monitor)
    {
        this.monitor = monitor;
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

    public void Dispose()
    {
        monitor.OnRecordChanged -= OnRecordChanged;
    }
}
