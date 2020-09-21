using System;
using System.Threading;

namespace RDRN_Core.Streamer
{
    public partial class SyncCollector : Script
    {
        internal static bool ForceAimData;
        internal static object LastSyncPacket;
        internal static object Lock = new object();

        public SyncCollector()
        {
            var t = new Thread(SyncSender.MainLoop) {IsBackground = true};
            t.Start();
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!Main.IsOnServer) 
                return;

            var player = Game.Player.Character;

            if (player.IsInVehicle())
            {
                VehicleData(player);
            }
            else
            {
                PedData(player);
            }
        }
    }
}
