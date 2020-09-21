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

        [Key(23)]
        public int PrimaryColor { get; set; }

        [Key(24)]
        public int SecondaryColor { get; set; }

        [Key(25)]
        public float Health { get; set; }

        [Key(26)]
        public bool IsDead { get; set; }

        [Key(27)]
        public Dictionary<byte, int> Mods { get; set; }

        [Key(28)]
        public bool Siren { get; set; }

        [Key(29)]
        public byte Doors { get; set; }

        [Key(30)]
        public int Trailer { get; set; }

        [Key(31)]
        public byte Tires { get; set; }

        [Key(32)]
        public int Livery { get; set; }

        [Key(33)]
        public string NumberPlate { get; set; }

        [Key(34)]
        public short VehicleComponents { get; set; }

        [Key(35)]
        public int TraileredBy { get; set; }

        [Key(36)]
        public VehicleDamageModel DamageModel { get; set; }
    }
}
