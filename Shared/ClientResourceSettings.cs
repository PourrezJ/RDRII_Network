using ProtoBuf;
using System.Collections.Generic;

namespace Shared
{
    [ProtoContract]
    public class ClientResourceSettings
    {
        [ProtoMember(1)]
        public Dictionary<string, NativeArgument> Settings { get; set; }
    }
}
