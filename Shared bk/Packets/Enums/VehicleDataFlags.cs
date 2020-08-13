using System;

namespace Shared.Packets.Enums
{
    [Flags]
    public enum VehicleDataFlags
    {
        PressingHorn = 1 << 0,
        Shooting = 1 << 1,
        SirenActive = 1 << 2,
        VehicleDead = 1 << 3,
        Aiming = 1 << 4,
        Driver = 1 << 5,
        HasAimData = 1 << 6,
        BurnOut = 1 << 7,
        ExitingVehicle = 1 << 8,
        PlayerDead = 1 << 9,
        Braking = 1 << 10,
    }
}
