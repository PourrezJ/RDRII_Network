using System.Collections.Generic;
using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class SharedSettings
    {
        [Key(0)]
        public bool VehicleLagCompensation { get; set; }

        [Key(1)]
        public bool OnFootLagCompensation { get; set; }

        [Key(2)]
        public List<string> ModWhitelist { get; set; }

        [Key(3)]
        public bool UseHttpServer { get; set; }

        [Key(4)]
        public int PlayerStreamingRange { get; set; }

        [Key(5)]
        public int VehicleStreamingRange { get; set; }

        [Key(6)]
        public int GlobalStreamingRange { get; set; }
    }
}