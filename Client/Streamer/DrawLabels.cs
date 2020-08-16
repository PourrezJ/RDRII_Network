using System;

namespace RDRN_Core.Streamer
{
    public class DrawLabels : Script
    {
        public DrawLabels()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected) 
                Main.NetEntityHandler.DrawLabels();
        }
    }
}
