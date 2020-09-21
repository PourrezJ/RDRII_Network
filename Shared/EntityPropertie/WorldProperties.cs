using System.Collections.Generic;
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class WorldProperties : EntityProperties
    {
        [Key(23)]
        public byte Hours { get; set; }

        [Key(24)]
        public byte Minutes { get; set; }

        [Key(25)]
        public int Weather { get; set; }

        [Key(26)]
        public List<string> LoadedIpl { get; set; }

        [Key(27)]
        public List<string> RemovedIpl { get; set; }
    }
}
