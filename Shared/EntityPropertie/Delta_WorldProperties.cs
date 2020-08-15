using System.Collections.Generic;
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class Delta_WorldProperties : Delta_EntityProperties
    {
        [Key(0)]
        public byte? Hours { get; set; }

        [Key(1)]
        public byte? Minutes { get; set; }

        [Key(2)]
        public string Weather { get; set; }

        [Key(3)]
        public List<string> LoadedIpl { get; set; }

        [Key(4)]
        public List<string> RemovedIpl { get; set; }
    }
}
