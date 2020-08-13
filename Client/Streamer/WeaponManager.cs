using System;
using System.Collections.Generic;
using System.Linq;
using RDRN_Core.Util;

namespace RDRN_Core.Streamer
{
    public class WeaponManager : Script
    {

        public WeaponManager()
        {
            Tick += OnTick;
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (Main.IsConnected())
            {
                Update();
            }
        }

        private static List<Shared.WeaponHash> _playerInventory = new List<Shared.WeaponHash>
        {
            Shared.WeaponHash.Unarmed
        };

        public void Clear()
        {
            _playerInventory.Clear();
            _playerInventory.Add(Shared.WeaponHash.Unarmed);
        }

        private static DateTime LastDateTime = DateTime.Now;
        internal static void Update()
        {
            if (DateTime.Now.Subtract(LastDateTime).TotalMilliseconds >= 500)
            {
                LastDateTime = DateTime.Now;
                var weapons = Enum.GetValues(typeof(Shared.WeaponHash)).Cast<Shared.WeaponHash>();
                foreach (var hash in weapons)
                {
                    if (!_playerInventory.Contains(hash) && hash != Shared.WeaponHash.Unarmed)
                    {
                        //Game.Player.Character.Weapons.Remove((RDR2.WeaponHash)(int)hash);
                    }
                }
            }

        }

        public void Allow(Shared.WeaponHash hash)
        {
            if (!_playerInventory.Contains(hash)) _playerInventory.Add(hash);
        }

        public void Deny(Shared.WeaponHash hash)
        {
            _playerInventory.Remove(hash);
        }
    }
}