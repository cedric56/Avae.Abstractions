namespace Avae.MagicLayer;

public interface IXmlHttpRequest
{
    byte[] Send(string urlString, byte[] data);
}
