using System;
using System.Drawing;
using System.Linq;

namespace RDRN_Core.Streamer
{
    public class DrawLocalMarkers : Script
    {
        public DrawLocalMarkers()
        {
            Tick += Draw;
        }

        private static void Draw(object sender, EventArgs e)
        {
            if (Main.IsConnected)
            {
                lock (Main._localMarkers)
                {
                    for (var index = Main._localMarkers.Count - 1; index >= 0; index--)
                    {
                        var marker = Main._localMarkers.ElementAt(index);
                        World.DrawMarker((MarkerType) marker.Value.MarkerType, marker.Value.Position,
                            marker.Value.Direction, marker.Value.Rotation,
                            marker.Value.Scale,
                            Color.FromArgb(marker.Value.Alpha, marker.Value.Red, marker.Value.Green, marker.Value.Blue), marker.Value.BobUpAndDown);
                    }
                }
            }
        }
    }
}
