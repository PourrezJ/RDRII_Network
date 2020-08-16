using System;

namespace RDRN_Core.Streamer
{
    public class DrawMarkers : Script
    {
        public DrawMarkers()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected)
                Main.NetEntityHandler.DrawMarkers();
        }
    }
}
