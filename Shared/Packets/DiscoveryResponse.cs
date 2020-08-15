using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class DiscoveryResponse
    {
        [Key(0)]
        public string ServerName { get; set; }
        [Key(1)]
        public short MaxPlayers { get; set; }
        [Key(2)]
        public short PlayerCount { get; set; }
        [Key(3)]
        public bool PasswordProtected { get; set; }
        [Key(4)]
        public int Port { get; set; }
        [Key(5)]
        public string Gamemode { get; set; }
        [Key(6)]
        public bool LAN { get; set; }
        [Key(7)]
        public string Map { get; set; }
    }
}