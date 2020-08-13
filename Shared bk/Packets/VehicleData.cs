using MessagePack;
using Shared.Math;

namespace Shared.Packets
{
    [MessagePackObject]
    public class VehicleData
    {
        [Key(1)]
        public Vector3 Position { get; set; }
        [Key(2)]
        public int? VehicleHandle { get; set; }
        [Key(3)]
        public int? NetHandle { get; set; }
        [Key(4)]
        public int? PedModelHash { get; set; }
        [Key(5)]
        public int? WeaponHash { get; set; }
        [Key(6)]
        public Vector3 Quaternion { get; set; }
        [Key(7)]
        public short? VehicleSeat { get; set; }
        [Key(8)]
        public float? VehicleHealth { get; set; }
        [Key(9)]
        public byte? PlayerHealth { get; set; }
        [Key(10)]
        public float? Latency { get; set; }
        [Key(11)]
        public Vector3 Velocity { get; set; }
        [Key(12)]
        public byte? PedArmor { get; set; }
        [Key(13)]
        public Vector3 AimCoords { get; set; }
        [Key(14)]
        public float? RPM { get; set; }
        [Key(15)]
        public short? Flag { get; set; }
        [Key(16)]
        public float? Steering { get; set; }
        [Key(17)]
        public Vector3 Trailer { get; set; }
        //[Key(18)]
        //public VehicleDamageModel DamageModel { get; set; }
    }
}
