using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class MarkerProperties : EntityProperties
    {
        public MarkerProperties()
        {
            EntityType = (byte)Shared.EntityType.Marker;
        }

        [Key(0)]
        public Vector3 Direction { get; set; }

        [Key(1)]
        public int MarkerType { get; set; }

        [Key(2)]
        public int Red { get; set; }

        [Key(3)]
        public int Green { get; set; }

        [Key(4)]
        public int Blue { get; set; }

        [Key(6)]
        public Vector3 Scale { get; set; }

        [Key(7)]
        public bool BobUpAndDown { get; set; }
    }
}
