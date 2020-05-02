using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
    public partial class Main
    {
        private static NetPeerConfiguration _config;

        internal static void PrepareNetwork()
        {
            _config = new NetPeerConfiguration("RDRNETWORK")
            {
                Port = 8888
            };
            _config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            _config.ConnectionTimeout = 30f; // 30 second timeout
        }
    }
}
