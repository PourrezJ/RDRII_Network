using MessagePack;
using System.Collections.Generic;

namespace Shared
{
    [MessagePackObject]
    public class SyncEvent
    {
        [Key(0)]
        public byte EventType { get; set; }

        [Key(1)]
        public List<NativeArgument> Arguments { get; set; }
    }
}