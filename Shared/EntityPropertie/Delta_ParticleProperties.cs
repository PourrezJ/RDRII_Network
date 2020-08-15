using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class Delta_ParticleProperties : Delta_EntityProperties
    {
        [Key(0)]
        public string Library { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public float? Scale { get; set; }

        [Key(3)]
        public int? EntityAttached { get; set; }

        [Key(4)]
        public int? BoneAttached { get; set; }
    }
}
