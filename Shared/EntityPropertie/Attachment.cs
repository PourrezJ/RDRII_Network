using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class Attachment
    {
        [Key(0)]
        public int NetHandle { get; set; }
        [Key(1)]
        public Vector3 PositionOffset { get; set; }
        [Key(2)]
        public Vector3 RotationOffset { get; set; }
        [Key(3)]
        public string Bone { get; set; }
    }
}
