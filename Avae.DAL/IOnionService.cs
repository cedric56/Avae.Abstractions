using Avae.Abstractions;
using MagicOnion;

namespace Avae.DAL
{
    public interface IOnionService
    {
        UnaryResult<byte[]> GetAllAsync(string type);

        UnaryResult<byte[]> FindByAnyAsync(string type, object filters);

    }

    public interface IXmlHttpRequest
    {
        byte[] Send(string url, string data);
    }
}
