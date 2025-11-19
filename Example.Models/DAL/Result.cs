using MemoryPack;
using MessagePack;

namespace Example.Models
{
    [MessagePackObject]
    [MemoryPackable]
    public partial class Result
    {
        [Key(0)]
        public string? Exception { get; set; }

        [Key(1)]
        public bool Success { get; set; }
    }
}
