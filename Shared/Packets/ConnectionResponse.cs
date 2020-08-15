using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class ConnectionResponse
    {
        [Key(0)]
        public int CharacterHandle { get; set; }

        [Key(1)]
        public SharedSettings Settings { get; set; }

        [Key(2)]
        public string ServerVersion { get; set; }
    }
}