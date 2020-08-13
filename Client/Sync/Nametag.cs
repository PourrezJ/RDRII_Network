﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RDRN_Core;
using RDRN_Core.Native;
using RDRN_Core.Javascript;
using RDRN_Core.Misc;
using RDRN_Core.Util;
using RDRN_Core.Streamer;
using Shared;
using Vector3 = Shared.Math.Vector3;
using WeaponHash = RDRN_Core.WeaponHash;
using VehicleHash = RDRN_Core.Native.VehicleHash;


namespace RDRN_Core.Sync
{
    public partial class SyncPed
    {

        //bool enteringSeat = _seatEnterStart != 0 && Util.Util.TickCount - _seatEnterStart < 500;
        //if ((enteringSeat || Character.IsSubtaskActive(67) || IsBeingControlledByScript || Character.IsExitingLeavingCar()))
        //{
        //    if (!Main.ToggleNametagDraw) DrawNametag();
        //    return;
        //}
        //if (!Main.ToggleNametagDraw) DrawNametag();
        /*
        internal void DrawNametag(CallCollection thisCollection)
        {
            if (!Main.UIVisible || Main._mainWarning.Visible) return;
            if ((NametagSettings & 1) != 0) return;

            // CallCollection thisCollection = new CallCollection();
            // && IsSpectating || (Flag & (int)EntityFlag.PlayerSpectating) != 0 ||

            if (!StreamedIn || ModelHash == 0 || string.IsNullOrEmpty(Name)) return;
            if(Character != null && Character.Exists())
            {
                var playerChar = Game.Player.Character;
                var isInRange = Character.IsInRangeOfEx(playerChar.Position, 30f);
                var isFreeAimngAtEntity = Function.Call<bool>(Hash.IS_PLAYER_FREE_AIMING_AT_ENTITY, Game.Player, Character);
                var hasClearLosToEntity = Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, playerChar, Character, 17);

                if ((isInRange || isFreeAimngAtEntity) && hasClearLosToEntity)
                {
                    var targetPos = Character.GetBoneCoord(Bone.IK_Head) + new Vector3(0, 0, 0.5f);

                    targetPos += Character.Velocity / Game.FPS;

                    thisCollection.Call(Hash.SET_DRAW_ORIGIN, targetPos.X, targetPos.Y, targetPos.Z, 0);;

                    var nameText = Name ?? "<nameless>";

                    if (!string.IsNullOrEmpty(NametagText))
                    {
                        nameText = NametagText;
                    }

                    if (TicksSinceLastUpdate > 10000)
                    {
                        nameText = "~r~AFK~w~~n~" + nameText;
                    }

                    var dist = (GameplayCamera.Position - Character.Position).Length();
                    var sizeOffset = Math.Max(1f - (dist / 30f), 0.3f);

                    var defaultColor = Color.FromArgb(245, 245, 245);

                    if ((NametagSettings & 2) != 0)
                    {
                        Util.Util.ToArgb(NametagSettings >> 8, out byte a, out byte r, out byte g, out byte b);
                        defaultColor = Color.FromArgb(r, g, b);
                    }

                    Util.Util.DrawText(nameText, 0, 0, 0.4f * sizeOffset, defaultColor.R, defaultColor.G, defaultColor.B, 255, 0, 1, false, true, 0, thisCollection);

                    var armorColor = Color.FromArgb(200, 220, 220, 220);
                    var bgColor = Color.FromArgb(100, 0, 0, 0);
                    var armorPercent = Math.Min(Math.Max(PedArmor / 100f, 0f), 1f);
                    var armorBar = Math.Round(150 * armorPercent);
                    armorBar = (armorBar * sizeOffset);
                    
                    Util.Util.DrawRectangle(-75 * sizeOffset, 36 * sizeOffset, armorBar, 20 * sizeOffset, armorColor.R, armorColor.G, armorColor.B, armorColor.A, thisCollection);
                    Util.Util.DrawRectangle(-75 * sizeOffset + armorBar, 36 * sizeOffset, (sizeOffset * 150) - armorBar, sizeOffset * 20, bgColor.R, bgColor.G, bgColor.B, bgColor.A, thisCollection);
                    Util.Util.DrawRectangle(-71 * sizeOffset, 40 * sizeOffset, (142 * Math.Min(Math.Max((PedHealth / 100f), 0f), 1f)) * sizeOffset, 12 * sizeOffset, 50, 250, 50, 150, thisCollection);

                    thisCollection.Call(Hash.CLEAR_DRAW_ORIGIN);
                }
            }
            
        }*/
    }
}
