using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class UpdateEntity
    {
        [Key(0)]
        public int NetHandle { get; set; }

        [Key(1)]
        public byte EntityType { get; set; }

        [Key(2)]
        public EntityPropertiesAbstract Properties { get; set; }
    }
}