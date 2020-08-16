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

        [Key(0)]
        public int Sprite { get; set; }

        [Key(1)]
        public float Scale { get; set; }

        [Key(2)]
        public int Color { get; set; }

        [Key(3)]
        public bool IsShortRange { get; set; }

        [Key(4)]
        public int AttachedNetEntity { get; set; }

        [Key(5)]
        public float RangedBlip { get; set; }

        [Key(6)]
        public string Name { get; set; }

        [Key(7)]
        public bool RouteVisible { get; set; }

        [Key(8)]
        public int RouteColor { get; set; }
    }
}
