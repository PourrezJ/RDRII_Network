using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class CreateEntity
    {
        [Key(0)]
        public int NetHandle { get; set; }

        [Key(1)]
        public byte EntityType { get; set; }

        [Key(2)]
        public EntityProperties Properties { get; set; }
    }
}