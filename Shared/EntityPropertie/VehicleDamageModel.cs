using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class VehicleDamageModel
    {
        [Key(23)]
        public byte BrokenWindows { get; set; }

        [Key(24)]
        public byte BrokenDoors { get; set; }

        [Key(25)]
        public int BrokenLights { get; set; }
    }
}
