using Avae.DAL;
using Avae.DAL.Interfaces;
using Grpc.Core;
using MagicOnion;

namespace Example.Models
{
    public interface IDBOnionService : IService<IDBOnionService>, IOnionService
    {
        UnaryResult<Result> DbTransRemove(DBModelBase modelBase);

        UnaryResult<Result> DbTransSave(DBModelBase modelBase);
    }

    public class DBOnionNotConnected : IDBOnionService
    {
        public bool IsConnected { get => false; set => throw new NotImplementedException(); }

        public UnaryResult<Result> DbTransRemove(DBModelBase modelBase)
        {
            throw new NotImplementedException();
        }

        public UnaryResult<Result> DbTransSave(DBModelBase modelBase)
        {
            throw new NotImplementedException();
        }

        public IDBOnionService WithCancellationToken(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IDBOnionService WithDeadline(DateTime deadline)
        {
            throw new NotImplementedException();
        }

        public IDBOnionService WithHeaders(Metadata headers)
        {
            throw new NotImplementedException();
        }

        public IDBOnionService WithHost(string host)
        {
            throw new NotImplementedException();
        }

        public IDBOnionService WithOptions(CallOptions option)
        {
            throw new NotImplementedException();
        }
    }
}
