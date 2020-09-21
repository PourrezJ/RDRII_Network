using System;
using System.Collections.Generic;
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class PlayerProperties : EntityProperties
    {
        public PlayerProperties()
        {
            Props = new Dictionary<byte, byte>();
            Textures = new Dictionary<byte, byte>();
            Accessories = new Dictionary<byte, Tuple<byte, byte>>();
            EntityType = (byte)Shared.EntityType.Player;
            WeaponTints = new Dictionary<int, byte>();
            WeaponComponents = new Dictionary<int, List<int>>();
        }

        [Key(23)]
        public Dictionary<byte, byte> Props { get; set; }

        [Key(24)]
        public Dictionary<byte, byte> Textures { get; set; }

        [Key(25)]
        public int BlipSprite { get; set; }

        [Key(26)]
        public int Team { get; set; }

        [Key(27)]
        public int BlipColor { get; set; }

        [Key(28)]
        public byte BlipAlpha { get; set; }

        [Key(29)]
        public Dictionary<byte, Tuple<byte, byte>> Accessories { get; set; }

        [Key(30)]
        public string Name { get; set; }

        [Key(31)]
        public Dictionary<int, byte> WeaponTints { get; set; }

        [Key(32)]
        public Dictionary<int, List<int>> WeaponComponents { get; set; } 

        [Key(33)]
        public string NametagText { get; set; }

        [Key(34)]
        public int NametagSettings { get; set; }   
    }
}
