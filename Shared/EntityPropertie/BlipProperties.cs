using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class BlipProperties : EntityPropertiesAbstract
    {
        public BlipProperties()
        {
            EntityType = (byte)Shared.EntityType.Blip;
        }

        [Key(23)]
        public int Sprite { get; set; }

        [Key(24)]
        public float Scale { get; set; }

        [Key(25)]
        public int Color { get; set; }

        [Key(26)]
        public bool IsShortRange { get; set; }

        [Key(27)]
        public int AttachedNetEntity { get; set; }

        [Key(28)]
        public float RangedBlip { get; set; }

        [Key(29)]
        public string Name { get; set; }

        [Key(30)]
        public bool RouteVisible { get; set; }

        [Key(31)]
        public int RouteColor { get; set; }
    }
}
