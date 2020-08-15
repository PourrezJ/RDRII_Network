using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class VehicleData
    {
        [Key(0)]
        public Vector3 Position { get; set; }
        [Key(1)]
        public int? VehicleHandle { get; set; }
        [Key(2)]
        public int? NetHandle { get; set; }
        [Key(3)]
        public int? PedModelHash { get; set; }
        [Key(4)]
        public int? WeaponHash { get; set; }
        [Key(5)]
        public Vector3 Quaternion { get; set; }
        [Key(6)]
        public short? VehicleSeat { get; set; }
        [Key(7)]
        public float? VehicleHealth { get; set; }
        [Key(8)]
        public byte? PlayerHealth { get; set; }
        [Key(9)]
        public float? Latency { get; set; }
        [Key(10)]
        public Vector3 Velocity { get; set; }
        [Key(11)]
        public byte? PedArmor { get; set; }
        [Key(12)]
        public Vector3 AimCoords { get; set; }
        [Key(13)]
        public float? RPM { get; set; }
        [Key(14)]
        public short? Flag { get; set; }
        [Key(15)]
        public float? Steering { get; set; }
        [Key(16)]
        public Vector3 Trailer { get; set; }
        [Key(17)]
        public VehicleDamageModel DamageModel { get; set; }
    }

}
