using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class DeleteEntity
    {
        [Key(0)]
        public int NetHandle { get; set; }
    }
}