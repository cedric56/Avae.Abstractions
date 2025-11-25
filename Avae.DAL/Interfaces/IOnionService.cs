using MagicOnion;

namespace Avae.DAL
{
    public interface IOnionService
    {
        UnaryResult<byte[]> GetAllAsync(string type);

        UnaryResult<byte[]> FindByAnyAsync(string type, Dictionary<string, object> filters);

        UnaryResult<byte[]> WhereAsync(string type, Dictionary<string, object> filters);

        UnaryResult<byte[]> GetAsync(string type, long id);
    }

    public interface IXmlHttpRequest
    {
        byte[]? Send(string url, string data);
    }
}
