using MessagePack;
using System.Collections.Generic;

namespace Shared.Packets
{
    [MessagePackObject]
    public class ScriptCollection
    {
        [Key(1)]
        public List<ClientsideScript> ClientsideScripts { get; set; }
    }
}
