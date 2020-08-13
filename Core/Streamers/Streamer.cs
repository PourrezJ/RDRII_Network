using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core.Streamer
{
    internal partial class Streamer
    {
        Streamer()
        {

        }
        /*
        internal SyncPed GetPlayer(int netHandle)
        {
            SyncPed rem = NetToStreamedItem(netHandle) as SyncPed;
            if (rem != null) return rem;

            rem = new SyncPed()
            {
                RemoteHandle = netHandle,
                EntityType = (byte)EntityType.Player,
                StreamedIn = false, // change me
                LocalOnly = false,

                BlipSprite = -1,
                BlipColor = -1,
                BlipAlpha = 255,
                Alpha = 255,
                Team = -1,
            };

            lock (ClientMap)
            {
                ClientMap.Add(netHandle, rem);
            }
            return rem;
        }*/
    }
}
