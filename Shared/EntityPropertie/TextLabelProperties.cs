using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class TextLabelProperties : EntityProperties
    {
        public TextLabelProperties()
        {
            EntityType = (byte) Shared.EntityType.TextLabel;
        }

        [Key(0)]
        public string Text { get; set; }

        [Key(1)]
        public int Red { get; set; }

        [Key(2)]
        public int Green { get; set; }

        [Key(3)]
        public int Blue { get; set; }

        [Key(4)]
        public float Size { get; set; }

        [Key(5)]
        public float Range { get; set; }

        [Key(6)]
        public bool EntitySeethrough { get; set; }
    }
}
