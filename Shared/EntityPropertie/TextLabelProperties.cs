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

        [Key(23)]
        public string Text { get; set; }

        [Key(24)]
        public int Red { get; set; }

        [Key(25)]
        public int Green { get; set; }

        [Key(26)]
        public int Blue { get; set; }

        [Key(27)]
        public float Size { get; set; }

        [Key(28)]
        public float Range { get; set; }

        [Key(29)]
        public bool EntitySeethrough { get; set; }
    }
}
