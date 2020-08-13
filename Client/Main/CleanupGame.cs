using System;
using RDRN_Core.Native;

namespace RDRN_Core
{
    public class CleanupGame : Script
    {
        public CleanupGame() {
            Tick += OnTick; 
        }

        private static DateTime LastDateTime = DateTime.Now;

        private static void OnTick(object sender, EventArgs e)
        {
            if (Main.IsConnected())
            {
                //CallCollection thisCol = new CallCollection();

                //thisCol.Call((Hash) 0xB96B00E976BE977F, 0.0); //_SET_WAVES_INTENSITY

                Function.Call(Hash.SET_RANDOM_TRAINS, 0);
                //Function.Call(Hash.CAN_CREATE_RANDOM_COPS, false);

                //Function.Call(Hash.SET_NUMBER_OF_PARKED_VEHICLES, -1);
                Function.Call(Hash.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0f);

                //if (Main.RemoveGameEntities)
                //{
                //Function.Call(Hash.SET_PED_POPULATION_BUDGET, 0);
               // Function.Call(Hash.SET_VEHICLE_POPULATION_BUDGET, 0);

                Function.Call(Hash.SUPPRESS_SHOCKING_EVENTS_NEXT_FRAME);
                //Function.Call(Hash.SUPPRESS_AGITATION_EVENTS_NEXT_FRAME);

                //Function.Call(Hash.SET_FAR_DRAW_VEHICLES, false);
                //Function.Call((Hash)0xF796359A959DF65D, false); // _DISPLAY_DISTANT_VEHICLES
                //Function.Call(Hash.SET_ALL_LOW_PRIORITY_VEHICLE_GENERATORS_ACTIVE, false);

                Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0f);
                Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0f);
               // Function.Call(Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0f);
                Function.Call(Hash.SET_SCENARIO_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0f, 0f);
                //}


                //Function.Call(Hash.SET_CAN_ATTACK_FRIENDLY, PlayerChar, true, true);
                //Function.Call(Hash.SET_PED_CAN_BE_TARGETTED, PlayerChar, true);

                //Function.Call((Hash)0xD2B315B6689D537D, Game.Player, false); //Some secret ingredient

                //Function.Call(Hash.SET_POLICE_IGNORE_PLAYER, playerChar, true);

                //Function.Call(Hash.SET_RANDOM_EVENT_FLAG, 0);
                //Function.Call(Hash.SET_MISSION_FLAG, Game.Player.Character, 0);
                ////Function.Call(Hash._RESET_LOCALPLAYER_STATE);
                //Function.Call(Hash.SET_RANDOM_EVENT_FLAG, 0);

                //Function.Call(Hash.DESTROY_MOBILE_PHONE);
               // Function.Call((Hash) 0x015C49A93E3E086E, true); //_DISABLE_PHONE_THIS_FRAME
                //Function.Call(Hash.DISPLAY_CASH, false);

                //Function.Call(Hash.SET_AUTO_GIVE_PARACHUTE_WHEN_ENTER_PLANE, Game.Player, false);

                //Function.Call(Hash.HIDE_HELP_TEXT_THIS_FRAME);
                //Function.Call((Hash) 0x5DB660B38DD98A31, Game.Player, 0f); //SET_PLAYER_HEALTH_RECHARGE_MULTIPLIER

                Function.Call(Hash.SET_PLAYER_WANTED_LEVEL, Game.Player, 0, false);
                //Function.Call(Hash.SET_PLAYER_WANTED_LEVEL_NOW, Game.Player, false);
                Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);

                //if (Function.Call<bool>(Hash.IS_STUNT_JUMP_IN_PROGRESS)) thisCol.Call(Hash.CANCEL_STUNT_JUMP);

                //thisCol.Execute();

                //if (!Main.RemoveGameEntities) return;

                if (DateTime.Now.Subtract(LastDateTime).TotalMilliseconds >= 500)
                {
                    var playerChar = Game.Player.Character;

                    LastDateTime = DateTime.Now;
                    /*
                    foreach (var entity in World.GetAllPeds())
                    {
                        if (!Main.NetEntityHandler.ContainsLocalHandle(entity.Handle) && entity != playerChar)
                        {
                            entity.MarkAsNoLongerNeeded();
                            entity.Health = -1; //"Some special peds like Epsilon guys or seashark minigame will refuse to despawn if you don't kill them first." - Guad
                            entity.Delete();
                        }
                    }

                    foreach (var entity in World.GetAllVehicles())
                    {
                        var veh = Main.NetEntityHandler.NetToStreamedItem(entity.Handle, useGameHandle: true) as RemoteVehicle;
                        if (veh == null)
                        {
                            entity.MarkAsNoLongerNeeded();
                            entity.Delete();
                        }
                    }*/
                }
            }
        }
    }
}
