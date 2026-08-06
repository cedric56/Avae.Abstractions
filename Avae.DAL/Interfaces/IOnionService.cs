using MagicOnion;

namespace Avae.DAL.Interfaces
{
    public interface IOnionService
    {
        UnaryResult<Result> FindByAnyAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);
        
        UnaryResult<Result> GetAllAsync(string type, int? commandTimeout = null);
        
        UnaryResult<Result> GetAsync(string type, long id, int? commandTimeout = null);
        
        UnaryResult<Result> WhereAsync(string type, Dictionary<string, object> filters, int? commandTimeout = null);        
    }

    public interface IXmlHttpRequest
    {
        bool IsConnected { get; set; }
        Result Send(string url, string data);
    }
}
