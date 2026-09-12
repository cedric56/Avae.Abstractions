using Avae.DAL;

namespace Example.Models;

public class Repository : IDisposable
{
    private static readonly object _lock = new();

    private static Repository? _instance = null;
    public static Repository Instance
    {
        get
        {
            return _instance ?? throw new InvalidOperationException("Repository not initialized");
        }
    }

    public static void Initialize(IDBMonitor<Person> monitor)
    {
        lock (_lock)
        {
            _instance ??= new Repository(monitor);
        }
    }

    private readonly IDBMonitor<Person>? personMonitor;

    private Repository(IDBMonitor<Person> monitor)
    {
        personMonitor = monitor;
        personMonitor?.OnRecordChanged += Monitor_OnChanged;
    }

    private async void Monitor_OnChanged(object? sender, Record<Person> e)
    {
        await ClearPersons();
    }

    private IEnumerable<Person>? _persons;

    public IEnumerable<Person> Persons
    {
        get
        {
            _persons ??= DBBase.Instance.GetAll<Person>();
            return _persons ?? [];
        }
    }

    public event EventHandler<EventArgs>? PersonsChanged;

    public async Task ClearPersons()
    {
        _persons = await DBBase.Instance.GetAllAsync<Person>();
        PersonsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        personMonitor?.OnRecordChanged -= Monitor_OnChanged;
    }
}
