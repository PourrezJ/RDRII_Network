using RDRN_Core.Native;

namespace RDRN_Core
{

    public partial class Main
    {
        private static void ClearLocalEntities()
        {
            lock (EntityCleanup)
            {
                for (var index = EntityCleanup.Count - 1; index >= 0; index--)
                {
                    var prop = new Prop(EntityCleanup[index]);
                    if (prop.Exists()) prop.Delete();
                }
                EntityCleanup.Clear();
            }
        }

        private static void ClearLocalBlips()
        {
            lock (BlipCleanup)
            {
                for (var index = BlipCleanup.Count - 1; index >= 0; index--)
                {
                    var b = new Blip(BlipCleanup[index]);
                    if (b.Exists()) b.Delete();
                }
                BlipCleanup.Clear();
            }
        }

        private static void ResetPlayer()
        {
            var playerChar = Game.Player.Character;

            //playerChar.Position = _vinewoodSign;
            playerChar.FreezePosition = false;

            CustomAnimation = null;
            AnimationFlag = 0;

            Util.Util.SetPlayerSkin(PedHash.Player_Three);

            playerChar = Game.Player.Character;
            var player = Game.Player;

            playerChar.Health = 200;
            //playerChar.Style.SetDefaultClothes();

            playerChar.FreezePosition = false;
            player.IsInvincible = false;
            //playerChar.IsCollisionEnabled = true;
            playerChar.Alpha = 255;
            playerChar.IsInvincible = false;
            playerChar.Weapons.RemoveAll();
            //Function.Call(Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER, player, 1f);
            Function.Call(Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER, player, 1f);

            //Function.Call(Hash.SET_FAKE_WANTED_LEVEL, 0);
            Function.Call(Hash.DETACH_ENTITY, playerChar.Handle, true, true);
        }

        private static void ResetWorld()
        {
            World.RenderingCamera = MainMenuCamera;
            //MainMenu.Visible = true;
           // MainMenu.TemporarilyHidden = false;
            IsSpectating = false;
            Weather = null;
            Time = null;
            LocalTeam = -1;
            LocalDimension = 0;

            //Script.Wait(500);
            //PlayerChar.SetDefaultClothes();
        }

        private static void ClearStats()
        {
            BytesReceived = 0;
            BytesSent = 0;
            MessagesReceived = 0;
            MessagesSent = 0;
        }
    }
}
