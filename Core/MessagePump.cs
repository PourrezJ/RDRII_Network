using Lidgren.Network;
using RDRN_Core;
using System.Collections.Generic;

namespace RDRNetwork
{
    internal class MessagePump : Script
    {
        public MessagePump()
        {
            Tick += (sender, args) =>
            {
                if (Main.NetClient != null)
                {
                    var messages = new List<NetIncomingMessage>();
                    var msgsRead = Main.NetClient.ReadMessages(messages);
                    if (msgsRead > 0)
                    {
                        var count = messages.Count;
                        for (var i = 0; i < count; i++)
                        {
                           // Main.ProcessMessages(messages[i], true);
                        }
                    }
                }
            };
        }
    }
}
