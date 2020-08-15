using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class PlayerDisconnect
    {
        [Key(0)]
        public int Id { get; set; }
    }
}