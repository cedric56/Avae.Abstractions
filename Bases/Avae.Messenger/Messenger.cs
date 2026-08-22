using System.Collections.Concurrent;

namespace Avae.Messenger;

public class Messenger
{
    private static readonly object _lock = new();
    private static readonly ConcurrentDictionary<MessengerKey, object> Dictionary = new();

    private static Messenger? _instance = null;

    /// <summary>
    /// Gets the single instance of the Messenger.
    /// </summary>
    public static Messenger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Messenger();
                    }
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the Messenger class.
    /// </summary>
    private Messenger()
    {
    }

    /// <summary>
    /// Registers a recipient for a type of message T. The action parameter will be executed
    /// when a corresponding message is sent.
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""recipient""></param>
    /// <param name=""action""></param>
    public void Register<T>(object recipient, Action<Message<T>> action)
    {
        Register(recipient, action, null!);
    }

    /// <summary>
    /// Registers a recipient for a type of message T and a matching context. The action parameter will be executed
    /// when a corresponding message is sent.
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""recipient""></param>
    /// <param name=""action""></param>
    /// <param name=""token""></param>
    public void Register<T>(object recipient, Action<Message<T>> action, object token, string? key = null)
    {
        var m = new MessengerKey(recipient, token, key);
        Dictionary.TryAdd(m, action);
    }

    /// <summary>
    /// Unregisters a messenger recipient completely. After this method is executed, the recipient will
    /// no longer receive any messages.
    /// </summary>
    /// <param name=""recipient""></param>
    public void Unregister(object recipient)
    {
        Unregister(recipient, null!);
    }

    /// <summary>
    /// Unregisters a messenger recipient with a matching context completely. After this method is executed, the recipient will
    /// no longer receive any messages.
    /// </summary>
    /// <param name=""recipient""></param>
    /// <param name=""context""></param>
    public void Unregister(object recipient, object context)
    {
        var key = new MessengerKey(recipient, context);
        Dictionary.TryRemove(key, out _);
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will reach all recipients that are
    /// registered for this message type.
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""message""></param>
    public void Send<T>(T message)
    {
        Send(message, null!, null, null);
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will reach all recipients that are
    /// registered for this message type and matching context.
    /// </summary>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""message""></param>
    /// <param name=""token""></param>
    public void Send<T>(T message, object token, string? key = null, Delegate? callback = null)
    {
        IEnumerable<KeyValuePair<MessengerKey, object>> result;

        if (token == null)
        {
            // Get all recipients where the context is null.
            result = from r in Dictionary where r.Key.Token == null select r;
        }
        else
        {
            // Get all recipients where the context is matching.
            result = from r in Dictionary where r.Key.Token != null && r.Key.Token.Equals(token) && (r.Key.Key is null && key is null || true == r.Key.Key?.Equals(key)) select r;
        }

        foreach (var action in result.Select(x => x.Value))
        {
            if (action is Action<Message<T>> m)
                // Send the message to all recipients.
                m(new Message<T>() { Object = message, Callback = callback, Key = key });
            else if (action is Action<T> a)
                // Send the message to all recipients.
                a(message);
        }
    }

    protected class MessengerKey
    {
        public object Recipient { get; private set; }
        public object Token { get; private set; }

        public string? Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MessengerKey class.
        /// </summary>
        /// <param name=""recipient""></param>
        /// <param name=""token""></param>
        public MessengerKey(object recipient, object token, string? key = null)
        {
            Recipient = recipient;
            Token = token;
            Key = key;
        }

        /// <summary>
        /// Determines whether the specified MessengerKey is equal to the current MessengerKey.
        /// </summary>
        /// <param name=""other""></param>
        /// <returns></returns>
        protected bool Equals(MessengerKey other)
        {
            return Equals(Recipient, other.Recipient) && Equals(Token, other.Token) && Equals(Key, other.Key);
        }

        /// <summary>
        /// Determines whether the specified MessengerKey is equal to the current MessengerKey.
        /// </summary>
        /// <param name=""obj""></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((MessengerKey)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Recipient != null ? Recipient.GetHashCode() : 0) * 397 ^ (Token != null ? Token.GetHashCode() : 0);
            }
        }
    }
}