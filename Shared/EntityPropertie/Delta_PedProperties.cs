using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class Delta_PedProperties : Delta_EntityProperties
    {
        [Key(0)]
        public string LoopingAnimation { get; set; }
    }

}
