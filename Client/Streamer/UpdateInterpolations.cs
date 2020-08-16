using System;

namespace RDRN_Core.Streamer
{
    public class UpdateInterpolations : Script
    {
        public UpdateInterpolations()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected) 
                Main.NetEntityHandler.UpdateInterpolations();
        }
    }
}
