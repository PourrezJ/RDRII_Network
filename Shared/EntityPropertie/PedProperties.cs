using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class PedProperties : EntityProperties
    {
        public PedProperties()
        {
            EntityType = (byte)Shared.EntityType.Player;
        }

        [Key(0)]
        public string LoopingAnimation { get; set; }
    }
}
