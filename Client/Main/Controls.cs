using System;

namespace RDRN_Core
{
    public class Controls : Script
    {
        public Controls() 
        { 
            Tick += OnTick; 
        }

        private static void OnTick(object sender, EventArgs e)
        {
            Game.DisableControlThisFrame(0, Control.FrontendSocialClub);
            Game.DisableControlThisFrame(0, Control.FrontendSocialClubSecondary);
            Game.DisableControlThisFrame(0, Control.EnterCheatCode);

            Game.DisableControlThisFrame(0, Control.SpecialAbility);
            //Game.DisableControlThisFrame(0, Control.SpecialAbilityPC);
            Game.DisableControlThisFrame(0, Control.SpecialAbilitySecondary);
            Game.DisableControlThisFrame(0, Control.CharacterWheel);
            Game.DisableControlThisFrame(0, Control.Phone);
            Game.DisableControlThisFrame(0, Control.Duck);

            if (Main.IsConnected())
            {
                Game.DisableControlThisFrame(0, Control.FrontendPause);
                Game.DisableControlThisFrame(0, Control.FrontendPauseAlternate);
            }

            var playerChar = Game.Player.Character;
            if (playerChar.IsJumping)
            {
                //Game.DisableControlThisFrame(Control.MeleeAttack1);
                Game.DisableControlThisFrame(0, Control.MeleeAttack);
                //thisCol.Call(Hash.DISABLE_CONTROL_ACTION, 0, Control.MeleeAttackLight, true);

            }

            if (playerChar.IsRagdoll)
            {
                Game.DisableControlThisFrame(0, Control.Attack);
                Game.DisableControlThisFrame(0, Control.Attack2);
            }

            if (Game.IsControlPressed(0, Control.Aim) /* && !playerChar.IsInVehicle() && playerChar.Weapons.Current.Hash != WeaponHash.Unarmed*/)
            {
                //Game.DisableControlThisFrame(0, Control.Jump);
            }

            //CRASH WORKAROUND: DISABLE PARACHUTE RUINER2
            if (playerChar.IsInVehicle())
            {
                if (playerChar.CurrentVehicle.IsInAir && playerChar.CurrentVehicle.Model.Hash == 941494461)
                {
                    Game.DisableAllControlsThisFrame(0);
                }
            }
            /*
            if (Function.Call<int>(Hash.GET_PED_PARACHUTE_STATE, playerChar) == 2)
            {
                thisCol.Call(Hash.DISABLE_CONTROL_ACTION, 0, Control.Aim, true);
                thisCol.Call(Hash.DISABLE_CONTROL_ACTION, 0, Control.Attack, true);

                //Game.DisableControlThisFrame(Control.Aim);
                //Game.DisableControlThisFrame(Control.Attack);
            }*/
            //thisCol.Execute();
        }
    }
}
