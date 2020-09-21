using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class PickupProperties : EntityProperties
    {
        public PickupProperties()
        {
            EntityType = (byte)Shared.EntityType.Pickup;
        }

        [Key(24)]
        public int Amount { get; set; }

        [Key(25)]
        public bool PickedUp { get; set; }

        [Key(26)]
        public uint RespawnTime { get; set; }

        [Key(27)]
        public int CustomModel { get; set; }
    }
}
