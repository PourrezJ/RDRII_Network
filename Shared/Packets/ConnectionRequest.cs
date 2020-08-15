using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class ConnectionRequest
    {
        [Key(0)]
        public string SocialClubName { get; set; }

        [Key(1)]
        public string Password { get; set; }

        [Key(2)]
        public string DisplayName { get; set; }

        [Key(3)]
        public byte GameVersion { get; set; }

        [Key(4)]
        public string ScriptVersion { get; set; }

        [Key(5)]
        public bool CEF { get; set; }

        [Key(6)]
        public bool CEFDevtool { get; set; }

        [Key(7)]
        public bool MediaStream { get; set; }
    }
}