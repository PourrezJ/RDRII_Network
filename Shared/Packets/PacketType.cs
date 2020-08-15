using System;

namespace Shared
{
    public enum PacketType
    {
        //VehiclePositionData = 0,
        ChatData = 1,
        PlayerDisconnect = 2,
        //PedPositionData = 3,
        NpcVehPositionData = 4,
        ConnectionPacket = 5,
        WorldSharingStop = 6,
        DiscoveryResponse = 7,
        ConnectionRequest = 8,
        NativeCall = 9,
        NativeResponse = 10,
        PlayerRespawned = 11,
        NativeTick = 12,
        NativeTickRecall = 13,
        NativeOnDisconnect = 14,
        NativeOnDisconnectRecall = 15,
        CreateEntity = 16,
        DeleteEntity = 17,
        ScriptEventTrigger = 18,
        SyncEvent = 19,
        FileTransferTick = 20,
        FileTransferRequest = 21,
        FileTransferComplete = 22,
        ConnectionConfirmed = 23,
        PlayerKilled = 24,
        StopResource = 25,
        UpdateEntityProperties = 26,
        FileAcceptDeny = 27,
        ServerEvent = 28,
        RedownloadManifest = 29,
        PedPureSync = 30,
        PedLightSync = 31,
        VehiclePureSync = 32,
        VehicleLightSync = 33,
        BasicSync = 34,
        BulletSync = 35,
        UnoccupiedVehStartStopSync = 36,
        UnoccupiedVehSync = 37,
        BasicUnoccupiedVehSync = 38,
        BulletPlayerSync = 39,
        DeleteObject = 40,
    }

    public enum ScriptVersion
    {
        Unknown = 0,
        VERSION_0_6 = 1,
        VERSION_0_6_1 = 2,
        VERSION_0_7 = 3,
        VERSION_0_8_1 = 4,
        VERSION_0_9 = 5,
    }    

    public enum FileType
    {
        Normal = 0,
        Map = 1,
        Script = 2,
        EndOfTransfer = 3,
        CustomData = 4,
    }

    public enum SyncEventType
    {
        LandingGearChange = 0,
        DoorStateChange = 1,
        BooleanLights = 2,
        TrailerDeTach = 3,
        TireBurst = 4,
        RadioChange = 5,
        PickupPickedUp = 6,
        StickyBombDetonation = 7,
    }

    public enum ServerEventType
    {
        PlayerTeamChange = 0,
        PlayerBlipColorChange = 1,
        PlayerBlipAlphaChange = 2,
        PlayerBlipSpriteChange = 3,
        PlayerSpectatorChange = 4,
        PlayerAnimationStart = 5,
        PlayerAnimationStop = 6,
        EntityDetachment = 7,
        WeaponPermissionChange = 8,
    }

    public enum Lights
    {
        NormalLights = 0,
        Highbeams = 1,
    }

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
    
    [Flags]
    public enum PedDataFlags
    {
        Jumping = 1 << 0,
        Shooting = 1 << 1,
        Aiming = 1 << 2,
        ParachuteOpen = 1 << 3,
        Ragdoll = 1 << 4,
        InMeleeCombat = 1 << 5,
        InFreefall = 1 << 6,
        IsInCover = 1 << 7,
        IsInLowerCover = 1 << 8,
        IsInCoverFacingLeft = 1 << 9,
        IsReloading = 1 << 10,
        HasAimData = 1 << 11,
        IsOnLadder = 1 << 12,
        IsVaulting = 1 << 13,
        EnteringVehicle = 1 << 14,
        ClosingVehicleDoor = 1 << 15,
        OnFire = 1 << 16,
        PlayerDead = 1 << 17,
    }

    public enum ConnectionChannel
    {
        Default = 0,
        FileTransfer = 1,
        NativeCall = 2,
        Chat = 3,
        EntityBackend = 4,
        ClientEvent = 5,
        SyncEvent = 6,
        PureSync = 7,
        LightSync = 8,
        BasicSync = 9,
        BulletSync = 10,
        UnoccupiedVeh = 11
    }
}