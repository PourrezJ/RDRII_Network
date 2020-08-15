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

        [Key(0)]
        public Dictionary<byte, byte> Props { get; set; }

        [Key(1)]
        public Dictionary<byte, byte> Textures { get; set; }

        [Key(2)]
        public int BlipSprite { get; set; }

        [Key(3)]
        public int Team { get; set; }

        [Key(4)]
        public int BlipColor { get; set; }

        [Key(5)]
        public byte BlipAlpha { get; set; }

        [Key(6)]
        public Dictionary<byte, Tuple<byte, byte>> Accessories { get; set; }

        [Key(7)]
        public string Name { get; set; }

        [Key(8)]
        public Dictionary<int, byte> WeaponTints { get; set; }

        [Key(9)]
        public Dictionary<int, List<int>> WeaponComponents { get; set; } 

        [Key(10)]
        public string NametagText { get; set; }

        [Key(11)]
        public int NametagSettings { get; set; }   
    }
}
