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

        [Key(23)]
        public Vector3 Direction { get; set; }

        [Key(24)]
        public int MarkerType { get; set; }

        [Key(25)]
        public int Red { get; set; }

        [Key(26)]
        public int Green { get; set; }

        [Key(27)]
        public int Blue { get; set; }

        [Key(28)]
        public Vector3 Scale { get; set; }

        [Key(29)]
        public bool BobUpAndDown { get; set; }
    }
}
