namespace Avae.Messenger;

public class Message<T>
{
    public Message()
    {

    }
    public Message(T obj)
    {
        Object = obj;
    }
    public string? Key { get; set; }
    public T? Object { get; set; }
    public Delegate? Callback { get; set; }
}
