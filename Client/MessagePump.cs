﻿using Lidgren.Network;
using System.Collections.Generic;

namespace RDRN_Core
{
    internal class MessagePump : Script
    {
        public MessagePump()
        {
            Tick += (sender, args) =>
            {
                if (Main.Client != null)
                {
                    var messages = new List<NetIncomingMessage>();
                    var msgsRead = Main.Client.ReadMessages(messages);
                    if (msgsRead > 0)
                    {
                        var count = messages.Count;
                        for (var i = 0; i < count; i++)
                        {
                            Main.Instance.ProcessMessages(messages[i], true);
                        }
                    }
                }
            };
        }
    }
}
