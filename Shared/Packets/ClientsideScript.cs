
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class ClientsideScript
    {
        [Key(0)]
        public string ResourceParent { get; set; }

        [Key(1)]
        public string Script { get; set; }

        [Key(2)]
        public string Filename { get; set; }

        [Key(3)]
        public string MD5Hash { get; set; }
    }
}