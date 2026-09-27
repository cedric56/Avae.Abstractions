using Avae.DAL;
using Dommel;

namespace Example.Models;

public class InMemoryEntityCache<T> : IEntityCache<T>, IDisposable where T : class, new()
{
    public event EventHandler<EventArgs>? EntitiesChanged;

    protected Dictionary<long, T> _byId = [];
    protected readonly Func<T, long> _id;
    protected readonly IDBMonitor<T>? _monitor;
    private readonly IEntityMapper? _mapper;
    public IEnumerable<T> Entities => _byId.Select(b => b.Value);

    protected InMemoryEntityCache(Func<T, long> id, IDBMonitor<T>? monitor = null)
    {
        _id = id;
        _monitor = monitor;
        _monitor?.OnRecordChanged += Monitor_OnChanged;
    }
    public InMemoryEntityCache(Func<T, long> id, IEntityMapper mapper, IDBMonitor<T>? monitor = null)
        : this(id, monitor)
    {
        _mapper = mapper;
    }

    public void Track(T entity) => _byId[_id(entity)] = entity;
    public T? Find(long id) => _byId.TryGetValue(id, out var e) ? e : default;

    private async void Monitor_OnChanged(object? sender, Record<T> e)
    {
        await ClearEntities();
    }

    public async Task ClearEntities()
    {
        await LoadEntities();
        EntitiesChanged?.Invoke(this, EventArgs.Empty);
    }

    public virtual async Task LoadEntities()
    {
        if (_mapper == null)
            return;

        using var connection = _mapper.CreateConnection();
        var entities = await connection.GetAllAsync<T>();
        _byId = entities.ToDictionary(p => _id(p), p => p);
    }

    public void Dispose()
    {
        _monitor?.OnRecordChanged -= Monitor_OnChanged;
    }
}
