using MessagePack;
using System.Collections.Generic;

namespace Shared
{
    [MessagePackObject]
    public class ClientResourceSettings
    {
        [Key(0)]
        public Dictionary<string, NativeArgument> Settings { get; set; }
    }
}