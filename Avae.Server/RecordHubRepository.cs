using Avae.DAL;
using Avae.MagicServices;
using MagicOnion.Server.Hubs;

namespace Avae.Server;

public class RecordHubRepository<TObject> where TObject : class, new()
{
    readonly Dictionary<Guid, byte> customerIds = new(); // just tracking membership, group itself is shared
    IGroup<IRecordHubReceiver<TObject>>? group;
    readonly object gate = new();

    public RecordHubRepository(IDBMonitor<TObject> monitor)
    {
        IDBFactory.Monitors.Add(monitor);
        monitor.OnRecordChanged += (_, e) => Raise(e); // subscribed exactly ONCE, for the app lifetime
    }

    // Called by each connecting hub instance; captures the shared group once.
    public void RegisterGroup(IGroup<IRecordHubReceiver<TObject>> g, Guid contextId)
    {
        lock (gate)
        {
            Console.WriteLine(contextId);
            group ??= g; // same underlying group object every time, but only need to capture it once
            customerIds[contextId] = 0;
        }
    }

    public void Unregister(Guid contextId)
    {
        lock (gate)
        {
            Console.WriteLine(contextId);
            customerIds.Remove(contextId);
        }
    }

    public void Raise(Record<TObject> e)
    {
        var excludedIds = (e.Connections ?? []).Select(id => new Guid(id)).ToList();

        List<Guid> notified;
        lock (gate)
        {
            notified = customerIds.Where(id => !excludedIds.Contains(id.Key)).Select(k => k.Key).ToList();
        }

        foreach (var id in notified)
            Console.WriteLine($"Notifying customer: {id}");

        group?.Except(excludedIds).OnChanged(e);
    }
}
