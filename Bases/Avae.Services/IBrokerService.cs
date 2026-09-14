using System.Collections.Concurrent;

namespace Avae.Services;

/// <summary>
/// A message published on the in-process broker.
/// </summary>
/// <param name="Topic">The topic/channel name subscribers listen on.</param>
/// <param name="Data">The message payload.</param>
public record Message(string Topic, string Data);

/// <summary>
/// A lightweight in-process publish/subscribe broker for decoupling
/// components (e.g. ViewModels, services) that need to communicate
/// without holding direct references to each other.
/// </summary>
public interface IBrokerService
{
    /// <summary>
    /// Publishes a message to every subscriber currently registered
    /// for <see cref="Message.Topic"/>. No-op if there are no subscribers.
    /// </summary>
    /// <param name="message">The message to dispatch.</param>
    void Publish(Message message);

    /// <summary>
    /// Registers a callback to be invoked whenever a message is published
    /// on the given <paramref name="topic"/>.
    /// </summary>
    /// <param name="topic">The topic to listen on.</param>
    /// <param name="callback">Invoked synchronously for each published message.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that unsubscribes <paramref name="callback"/>
    /// when disposed. Callers should hold onto and dispose this to avoid leaking
    /// the subscription for the lifetime of the broker.
    /// </returns>
    IDisposable Subscribe(string topic, Action<Message> callback);

    /// <summary>
    /// Removes a previously registered callback from <paramref name="topic"/>.
    /// No-op if the callback isn't currently subscribed.
    /// </summary>
    /// <param name="topic">The topic the callback was subscribed to.</param>
    /// <param name="callback">The callback instance to remove.</param>
    void Unsubscribe(string topic, Action<Message> callback);
}

/// <summary>
/// Default in-memory implementation of <see cref="IBrokerService"/>.
/// Thread-safe: subscribing, unsubscribing, and publishing can all be
/// called concurrently from multiple threads.
/// </summary>
public class BrokerService : IBrokerService
{
    // Maps each topic to the list of callbacks currently subscribed to it.
    // ConcurrentDictionary handles concurrent topic add/lookup; the inner
    // List<T> is separately guarded by _gate since List<T> itself isn't
    // safe for concurrent mutation/iteration.
    private readonly ConcurrentDictionary<string, List<Action<Message>>> _subscriptions = new();
    private readonly Lock _gate = new();

    /// <inheritdoc />
    public void Publish(Message message)
    {
        if (!_subscriptions.TryGetValue(message.Topic, out var callbacks))
            return;

        // Snapshot under the lock so we never iterate a list that's being
        // mutated by a concurrent Subscribe/Unsubscribe, and so a callback
        // that itself calls Subscribe/Unsubscribe can't deadlock or corrupt
        // the list mid-dispatch.
        Action<Message>[] snapshot;
        lock (_gate)
        {
            snapshot = [.. callbacks];
        }

        // Invoke subscribers in registration order, synchronously,
        // on the calling thread.
        foreach (var callback in snapshot)
        {
            callback(message);
        }
    }

    /// <inheritdoc />
    public IDisposable Subscribe(string topic, Action<Message> callback)
    {
        var callbacks = _subscriptions.GetOrAdd(topic, static _ => []);

        lock (_gate)
        {
            callbacks.Add(callback);
        }

        return new Subscription(this, topic, callback);
    }

    /// <inheritdoc />
    public void Unsubscribe(string topic, Action<Message> callback)
    {
        if (!_subscriptions.TryGetValue(topic, out var callbacks))
            return;

        lock (_gate)
        {
            callbacks.Remove(callback);
        }
    }

    // Lets callers do `using var sub = broker.Subscribe(...)` instead of
    // remembering to call Unsubscribe manually with the exact same delegate.
    private sealed class Subscription(IBrokerService broker, string topic, Action<Message> callback) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            broker.Unsubscribe(topic, callback);
        }
    }
}