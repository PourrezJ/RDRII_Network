using System.Collections.Generic;
using MessagePack;
using Shared.Math;

namespace Shared
{
    [MessagePackObject]
    public class NativeResponse
    {
        [Key(0)]
        public NativeArgument Response { get; set; }

        [Key(1)]
        public uint Id { get; set; }
    }

    [MessagePackObject]
    public class NativeTickCall
    {
        [Key(0)]
        public NativeData Native { get; set; }

        [Key(1)]
        public string Identifier { get; set; }
    }

    [MessagePackObject]
    public class ScriptEventTrigger
    {
        [Key(0)]
        public string EventName { get; set; }

        [Key(1)]
        public List<NativeArgument> Arguments { get; set; }

        [Key(2)]
        public string Resource { get; set; }
    }

    [MessagePackObject]
    public class NativeData
    {
        [Key(0)]
        public ulong Hash { get; set; }

        [Key(1)]
        public List<NativeArgument> Arguments { get; set; }

        [Key(2)]
        public NativeArgument ReturnType { get; set; }

        [Key(3)]
        public uint Id { get; set; }

        [Key(4)]
        public bool Internal { get; set; }
    }

    [MessagePackObject]
    public class ObjectData
    {
        [Key(0)]
        public Vector3 Position { get; set; }

        [Key(1)]
        public float Radius { get; set; }

        [Key(2)]
        public int modelHash { get; set; }
    }

    [MessagePackObject]
    [Union(2, typeof(IntArgument))]
    [Union(3, typeof(UIntArgument))]
    [Union(4, typeof(StringArgument))]
    [Union(5, typeof(FloatArgument))]
    [Union(6, typeof(BooleanArgument))]
    [Union(7, typeof(LocalPlayerArgument))]
    [Union(8, typeof(EntityArgument))]
    [Union(9, typeof(LocalGamePlayerArgument))]
    [Union(10, typeof(Vector3Argument))]
    [Union(11, typeof(EntityPointerArgument))]
    [Union(12, typeof(ListArgument))]
    public class NativeArgument
    {
        [Key(0)]
        public string Id { get; set; }
    }

    [MessagePackObject]
    public class ListArgument : NativeArgument
    {
        [Key(0)]
        public List<NativeArgument> Data { get; set; }
    }

    [MessagePackObject]
    public class LocalPlayerArgument : NativeArgument
    {
    }

    [MessagePackObject]
    public class LocalGamePlayerArgument : NativeArgument
    {
    }

    [MessagePackObject]
    public class OpponentPedHandleArgument : NativeArgument
    {
        public OpponentPedHandleArgument(long opponentHandle)
        {
            Data = opponentHandle;
        }

        public OpponentPedHandleArgument()
        {
        }

        [Key(0)]
        public long Data { get; set; }
    }

    [MessagePackObject]
    public class IntArgument : NativeArgument
    {
        [Key(0)]
        public int Data { get; set; }
    }

    [MessagePackObject]
    public class UIntArgument : NativeArgument
    {
        [Key(0)]
        public uint Data { get; set; }
    }

    [MessagePackObject]
    public class StringArgument : NativeArgument
    {
        [Key(0)]
        public string Data { get; set; }
    }

    [MessagePackObject]
    public class FloatArgument : NativeArgument
    {
        [Key(0)]
        public float Data { get; set; }
    }

    [MessagePackObject]
    public class BooleanArgument : NativeArgument
    {
        [Key(0)]
        public bool Data { get; set; }
    }


    [MessagePackObject]
    public class Vector3Argument : NativeArgument
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }
    }

    [MessagePackObject]
    public class EntityArgument : NativeArgument
    {
        public EntityArgument()
        {
        }

        public EntityArgument(int netHandle)
        {
            NetHandle = netHandle;
        }

        [Key(0)]
        public int NetHandle { get; set; }
    }

    [MessagePackObject]
    public class EntityPointerArgument : NativeArgument
    {
        public EntityPointerArgument(int netHandle)
        {
            NetHandle = netHandle;
        }

        public EntityPointerArgument()
        {
        }

        [Key(0)]
        public int NetHandle { get; set; }
    }
}