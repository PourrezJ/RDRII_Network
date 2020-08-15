using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class VehicleDamageModel
    {
        [Key(0)]
        public byte BrokenWindows { get; set; }

        [Key(1)]
        public byte BrokenDoors { get; set; }

        [Key(2)]
        public int BrokenLights { get; set; }
    }
}
