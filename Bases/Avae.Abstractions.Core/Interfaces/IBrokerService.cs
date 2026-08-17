namespace Avae.Abstractions
{
    public record Message(string Topic, string Data);

    public interface IBrokerService
    {
        void Publish(Message message);
        void Subscribe(string topic, Action<Message> callback);
    }
}
