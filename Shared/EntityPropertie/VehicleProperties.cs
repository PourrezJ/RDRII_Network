using System.Collections.Generic;
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class VehicleProperties : EntityProperties
    {
        public VehicleProperties()
        {
            Mods = new Dictionary<byte, int>();
            EntityType = (byte)Shared.EntityType.Vehicle;
        }

        [Key(0)]
        public int PrimaryColor { get; set; }

        [Key(1)]
        public int SecondaryColor { get; set; }

        [Key(2)]
        public float Health { get; set; }

        [Key(3)]
        public bool IsDead { get; set; }

        [Key(4)]
        public Dictionary<byte, int> Mods { get; set; }

        [Key(5)]
        public bool Siren { get; set; }

        [Key(6)]
        public byte Doors { get; set; }

        [Key(7)]
        public int Trailer { get; set; }

        [Key(8)]
        public byte Tires { get; set; }

        [Key(9)]
        public int Livery { get; set; }

        [Key(10)]
        public string NumberPlate { get; set; }

        [Key(11)]
        public short VehicleComponents { get; set; }

        [Key(12)]
        public int TraileredBy { get; set; }

        [Key(13)]
        public VehicleDamageModel DamageModel { get; set; }
    }
}
