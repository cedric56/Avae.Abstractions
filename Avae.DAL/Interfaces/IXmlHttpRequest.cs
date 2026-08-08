namespace Avae.DAL.Interfaces;

public interface IXmlHttpRequest
{
    byte[] Send(string urlString, byte[] data);
}
