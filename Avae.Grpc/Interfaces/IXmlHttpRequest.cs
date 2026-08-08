namespace Avae.Grpc;

public interface IXmlHttpRequest
{
    byte[] Send(string urlString, byte[] data);
}
