using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class Delta_PickupProperties : Delta_EntityProperties
    {
        [Key(0)]
        public int? Amount { get; set; }

        [Key(1)]
        public bool? PickedUp { get; set; }

        [Key(2)]
        public uint? RespawnTime { get; set; }

        [Key(3)]
        public int? CustomModel { get; set; }
    }
}
