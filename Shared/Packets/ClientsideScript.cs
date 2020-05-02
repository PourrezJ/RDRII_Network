using MessagePack;

namespace Shared.Packets
{
    [MessagePackObject]
    public class ClientsideScript
    {
        [Key(1)]
        public string ResourceParent { get; set; }

        [Key(2)]
        public string Script { get; set; }

        [Key(3)]
        public string Filename { get; set; }

        [Key(4)]
        public string MD5Hash { get; set; }
    }
}
