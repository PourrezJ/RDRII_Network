using MessagePack;

namespace Shared.Packets
{
    [MessagePackObject]
    public class DiscoveryResponse
    {
        [Key(1)]
        public string ServerName { get; set; }
        [Key(2)]
        public short MaxPlayers { get; set; }
        [Key(3)]
        public short PlayerCount { get; set; }
        [Key(4)]
        public bool PasswordProtected { get; set; }
        [Key(5)]
        public int Port { get; set; }
        [Key(6)]
        public string Gamemode { get; set; }
        [Key(7)]
        public bool LAN { get; set; }
        [Key(8)]
        public string Map { get; set; }
    }
}
