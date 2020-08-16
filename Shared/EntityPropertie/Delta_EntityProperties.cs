using System.Collections.Generic;
using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    /*
    [Union(14, typeof(Delta_VehicleProperties))]
    [Union(15, typeof(Delta_BlipProperties))]
    [Union(16, typeof(Delta_MarkerProperties))]
    [Union(17, typeof(Delta_PickupProperties))]
    [Union(18, typeof(Delta_PlayerProperties))]
    [Union(19, typeof(Delta_TextLabelProperties))]
    [Union(20, typeof(Delta_WorldProperties))]
    [Union(21, typeof(Delta_PedProperties))]
    [Union(22, typeof(Delta_ParticleProperties))]*/
    public class Delta_EntityProperties
    {
        [Key(0)]
        public Vector3 Position { get; set; }

        [Key(1)]
        public Vector3 Rotation { get; set; }

        [Key(2)]
        public int? ModelHash { get; set; }

        [Key(3)]
        public byte? EntityType { get; set; }

        [Key(4)]
        public byte? Alpha { get; set; }

        [Key(5)]
        public int? Dimension { get; set; }

        [Key(6)]
        public Attachment AttachedTo { get; set; }

        [Key(7)]
        public List<int> Attachables { get; set; }

        [Key(8)]
        public byte? Flag { get; set; }

        [Key(9)]
        public Dictionary<string, NativeArgument> SyncedProperties { get; set; }

        [Key(10)]
        public Movement PositionMovement { get; set; }

        [Key(11)]
        public Movement RotationMovement { get; set; }

        [Key(12)]
        public bool? IsInvincible { get; set; }
    }
}
