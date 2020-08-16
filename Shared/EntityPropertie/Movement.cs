using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class Movement
    {
        [Key(0)]
        public long Duration { get; set; }

        [Key(1)]
        public long Start { get; set; }

        [Key(2)]
        public Vector3 StartVector { get; set; }

        [Key(3)]
        public Vector3 EndVector { get; set; }

        [Key(4)]
        public long ServerStartTime { get; set; }
    }
}
