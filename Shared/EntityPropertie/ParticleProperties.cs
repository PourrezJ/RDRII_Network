using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class ParticleProperties : EntityProperties
    {
        public ParticleProperties()
        {
            EntityType = (byte) Shared.EntityType.Particle;
        }

        [Key(23)]
        public string Library { get; set; }

        [Key(24)]
        public string Name { get; set; }

        [Key(25)]
        public float Scale { get; set; }

        [Key(26)]
        public int EntityAttached { get; set; }

        [Key(27)]
        public int BoneAttached { get; set; }
    }
}
