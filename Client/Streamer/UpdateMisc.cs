using System;

namespace RDRN_Core.Streamer
{
    public class UpdateMisc : Script
    {
        public UpdateMisc()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected)
                Main.NetEntityHandler.UpdateMisc();
        }
    }
}
