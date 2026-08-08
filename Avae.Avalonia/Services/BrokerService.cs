using Avae.Abstractions;

namespace Avae.Implementations
{
    public class BrokerService : IBrokerService
    {
        private readonly Dictionary<string, List<Action<Message>>> _subscriptions = [];

        public void Publish(Message message)
        {
            if (_subscriptions.TryGetValue(message.Topic, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback(message);
                }
            }
        }

        public void Subscribe(string topic, Action<Message> callback)
        {
            if (!_subscriptions.TryGetValue(topic, out var value))
            {
                value = [];
                _subscriptions.Add(topic, value);
            }

            value.Add(callback);
        }
    }
}
