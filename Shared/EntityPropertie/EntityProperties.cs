using System.Collections.Generic;
using MessagePack;
using Shared.Math;

namespace Shared
{
    public enum EntityType
    {
        Vehicle = 1,
        Prop = 2,
        Blip = 3,
        Marker = 4,
        Pickup = 5,
        Player = 6,
        TextLabel = 7,
        Ped = 8,
        Particle = 9,

        World = 255,
    }

    public enum EntityFlag
    {
        Collisionless = 1 << 0,
        EngineOff = 1 << 1,
        SpecialLight = 1 << 2,
        PlayerSpectating = 1 << 3,
        VehicleLocked = 1 << 4,
    }

    [MessagePackObject]
    [Union(13, typeof(VehicleProperties))]
    [Union(14, typeof(BlipProperties))]
    [Union(15, typeof(MarkerProperties))]
    [Union(16, typeof(PickupProperties))]
    [Union(17, typeof(PlayerProperties))]
    [Union(18, typeof(TextLabelProperties))]
    [Union(19, typeof(WorldProperties))]
    [Union(20, typeof(PedProperties))]
    [Union(21, typeof(ParticleProperties))]
    public class EntityProperties
    {
        [Key(0)]
        public Vector3 Position { get; set; }

        [Key(1)]
        public Vector3 Rotation { get; set; }

        [Key(2)]
        public int ModelHash { get; set; }

        [Key(3)]
        public byte EntityType { get; set; }

        [Key(4)]
        public byte Alpha { get; set; }

        [Key(5)]
        public int Dimension { get; set; }

        [Key(6)]
        public Attachment AttachedTo { get; set; }

        [Key(7)]
        public List<int> Attachables { get; set; }

        [Key(8)]
        public byte Flag { get; set; }

        [Key(9)]
        public Dictionary<string, NativeArgument> SyncedProperties { get; set; }

        [Key(10)]
        public Movement PositionMovement { get; set; }

        [Key(11)]
        public Movement RotationMovement { get; set; }

        [Key(12)]
        public bool IsInvincible { get; set; }

        [Key(22)]
        public Vector3 Velocity { get; set; }
    }
}
