using MessagePack;
using System.Collections.Generic;

namespace Shared
{
    [MessagePackObject]
    public class ScriptCollection
    {
        [Key(0)]
        public List<ClientsideScript> ClientsideScripts { get; set; }
    }
}