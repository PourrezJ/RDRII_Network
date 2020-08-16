using System;

namespace RDRN_Core.Streamer
{
    public class UpdateAttachements : Script
    {
        public UpdateAttachements()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected)
                Main.NetEntityHandler.UpdateAttachments();
        }
    }
}
