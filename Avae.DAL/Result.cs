using MemoryPack;
using MessagePack;

namespace Avae.DAL
{
    [MessagePackObject]
    [MemoryPackable]
    public partial class Result
    {
        [Key(0)]
        public string? Exception { get; set; }

        [Key(1)]
        public bool Successful { get; set; }

        [Key(2)]
        public byte[]? Data { get; set; }
    }
}
