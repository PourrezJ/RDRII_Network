using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class PedData
    {
        [Key(0)]
        public Vector3 Position { get; set; }
        [Key(1)]
        public int? NetHandle { get; set; }
        [Key(2)]
        public int? PedModelHash { get; set; }
        [Key(3)]
        public Vector3 Quaternion { get; set; }
        [Key(4)]
        public Vector3 AimCoords { get; set; }
        [Key(5)]
        public int? WeaponHash { get; set; }
        [Key(6)]
        public byte? PlayerHealth { get; set; }
        [Key(7)]
        public float? Latency { get; set; }
        [Key(8)]
        public byte? Speed { get; set; }
        [Key(9)]
        public byte? PedArmor { get; set; }
        [Key(10)]
        public int? Flag { get; set; }
        [Key(11)]
        public Vector3 Velocity { get; set; }
        [Key(12)]
        public int? VehicleTryingToEnter { get; set; }
        [Key(13)]
        public sbyte? SeatTryingToEnter { get; set; }
        [Key(14)]
        public int? WeaponAmmo { get; set; }
    }

}
