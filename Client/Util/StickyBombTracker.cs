using System;
using RDRN_Core;
using RDRN_Core.Javascript;
using RDRN_Core.Streamer;
using Shared;
using WeaponHash = RDRN_Core.WeaponHash;
using VehicleHash = RDRN_Core.Native.VehicleHash;

namespace RDRN_Core.Util
{
    public class StickyBombTracker /*: Script*/
    {
        public StickyBombTracker()
        {
            //base.Tick += OnTick;
        }

        private bool _hasPlacedStickies;

        private void OnTick(object sender, EventArgs e)
        {
            /*
            Ped player = Game.Player.Character;
            if (player.IsShooting && player.Weapons.Current.Hash == (WeaponHash.StickyBomb))
            {
                _hasPlacedStickies = true;
            }

            if (Game.Player.IsDead)
            {
                _hasPlacedStickies = false;
            }

            if (Game.IsControlJustPressed(0,Control.Detonate) && _hasPlacedStickies)
            {
                SyncEventWatcher.SendSyncEvent(SyncEventType.StickyBombDetonation, Main.NetEntityHandler.EntityToNet(player.Handle));
                JavascriptHook.InvokeCustomEvent(api => api?.invokeonPlayerDetonateStickies());
                _hasPlacedStickies = false;
            }*/
        }
    }
}