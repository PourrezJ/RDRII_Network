using System;
using System.Collections.Generic;
using System.Linq;
using RDRN_Core;
using RDRN_Core.Util;
using Shared;
using Lidgren.Network;
using Vector3 = Shared.Math.Vector3;
using Shared.Math;

namespace RDRN_Core.Streamer
{
    public class UnoccupiedVehSync : Script
    {
        public UnoccupiedVehSync() { Tick += OnTick; 
        }

        private static long _lastTick;
        private static void OnTick(object sender, EventArgs e)
        {
            //CallCollection thisCol = new CallCollection();
            if (Main.IsConnected() && Util.Util.TickCount - _lastTick > 1000) // Save ressource
            {
                _lastTick = Util.Util.TickCount;
                if (StreamerThread.StreamedInVehicles == null || StreamerThread.StreamedInVehicles.Length == 0) return;

                RemoteVehicle[] myCars;
                lock (StreamerThread.StreamedInVehicles) { myCars = StreamerThread.StreamedInVehicles.Take(50).ToArray(); }

                for (var index = myCars.Length - 1; index >= 0; index--)
                {
                    var remoteEntity = myCars[index];
                    if (remoteEntity == null) continue;

                    var localEntity = new Vehicle(remoteEntity.LocalHandle);

                    var isEmpty = Util.Util.IsVehicleEmpty(localEntity);
                    var isIntepolating = Main.VehicleSyncManager.IsInterpolating(localEntity.Handle);
                    var isTrailered = remoteEntity.TraileredBy != 0;
                    var isSyncing = Main.VehicleSyncManager.IsSyncing(remoteEntity);
                    var isLastVeh = localEntity.Handle == Game.Player.LastVehicle?.Handle;
                    var isLastVehTimeout = DateTime.Now.Subtract(Events.LastCarEnter).TotalMilliseconds > 3000;
                    var isFar = localEntity.Position.DistanceToSquared(remoteEntity.Position) > 2f;

                    //if (Util.Util.IsVehicleEmpty(entity) && !Main.VehicleSyncManager.IsInterpolating(entity.Handle) &&
                    //    entity_.TraileredBy == 0 && !Main.VehicleSyncManager.IsSyncing(entity_) &&
                    //    ((entity.Handle == Game.Player.LastVehicle?.Handle && DateTime.Now.Subtract(Events.LastCarEnter).TotalMilliseconds > 3000) ||
                    //     entity.Handle != Game.Player.LastVehicle?.Handle))

                    if (isEmpty && !isIntepolating && !isTrailered && !isSyncing && isLastVeh && isFar)
                    {
                        localEntity.PositionNoOffset = remoteEntity.Position;
                        localEntity.Rotation = remoteEntity.Rotation;
                        //thisCol.Call(Hash.SET_ENTITY_ROTATION, localEntity, value.X, value.Y, value.Z, 2, 1);
                    }

                    //veh.Position = entity.Position.ToLVector();
                    //veh.Rotation = entity.Rotation.ToLVector();
                }
            }

        }

    }


    internal class UnoccupiedVehicleSync
    {
        private List<RemoteVehicle> SyncedVehicles = new List<RemoteVehicle>();
        private const int UNOCCUPIED_VEH_RATE = 400;
        private long _lastUpdate;

        private Dictionary<int, UnoccupiedVehicleInterpolator> Interpolations = new Dictionary<int, UnoccupiedVehicleInterpolator>();

        internal void Interpolate(int netHandle, int gameHandle, Vector3 newPos, Vector3 newVelocity, Vector3 newRotation)
        {
            if (!Interpolations.ContainsKey(netHandle))
            {
                var interp = new UnoccupiedVehicleInterpolator(gameHandle, netHandle);
                interp.SetTargetPosition(newPos, newVelocity, newRotation);

                Interpolations.Set(netHandle, interp);
            }
            else
            {
                Interpolations[netHandle].SetTargetPosition(newPos, newVelocity, newRotation);
            }
        }

        internal bool IsInterpolating(int gameHandle)
        {
            return Interpolations.Any(p => p.Value.GameHandle == gameHandle);
        }

        internal void StartSyncing(int vehicle)
        {
            var veh = Main.NetEntityHandler.NetToStreamedItem(vehicle) as RemoteVehicle;

            if (veh != null)
            {
                lock (SyncedVehicles)
                {
                    SyncedVehicles.Add(veh);
                }

                if (veh.StreamedIn)
                {
                    var ent = Main.NetEntityHandler.NetToEntity(veh);
                    if (ent != null)
                    {
                        ent.IsInvincible = veh.IsInvincible;
                    }
                }
            }
        }

        internal bool IsSyncing(RemoteVehicle veh)
        {
            lock (SyncedVehicles)
            {
                return SyncedVehicles.Contains(veh);
            }
        }

        internal void StopSyncing(int vehicle)
        {
            var veh = Main.NetEntityHandler.NetToStreamedItem(vehicle) as RemoteVehicle;

            if (veh != null)
            {
                lock (SyncedVehicles)
                {
                    SyncedVehicles.Remove(veh);
                }

                if (veh.StreamedIn)
                {
                    var ent = Main.NetEntityHandler.NetToEntity(veh);
                    if (ent != null) ent.IsInvincible = true;
                }
            }
        }

        internal void StopAll()
        {
            lock (SyncedVehicles)
            {
                SyncedVehicles.Clear();
            }
        }

        internal void Pulse()
        {
            if (Util.Util.TickCount - _lastUpdate > UNOCCUPIED_VEH_RATE)
            {
                _lastUpdate = Util.Util.TickCount;

                if (SyncedVehicles.Count > 0)
                {
                    var vehicleCount = 0;
                    var buffer = new List<byte>();

                    lock (SyncedVehicles)
                    {
                        foreach (var vehicle in SyncedVehicles)
                        {
                            var ent = Main.NetEntityHandler.NetToEntity(vehicle);

                            if (ent == null || !vehicle.StreamedIn) continue;

                            var dist = ent.Model.IsBoat ? ent.Position.DistanceToSquared2D(vehicle.Position) : ent.Position.DistanceToSquared(vehicle.Position);

                            var veh = new Vehicle(ent.Handle);

                            byte BrokenDoors = 0;
                            byte BrokenWindows = 0;

                            for (var i = 0; i < 8; i++)
                            {
                                /*
                                if (veh.Doors[(VehicleDoorIndex)i].IsBroken) BrokenDoors |= (byte)(1 << i);
                                if (!veh.Windows[(VehicleWindowIndex)i].IsIntact) BrokenWindows |= (byte)(1 << i);*/
                            }

                            var syncUnocVeh = dist > 2f || ent.Rotation.DistanceToSquared(vehicle.Rotation) > 2f ||
                                          Math.Abs(new Vehicle(ent.Handle).Health - vehicle.Health) > 1f ||
                                          Util.Util.BuildTyreFlag(new Vehicle(ent.Handle)) != vehicle.Tires ||
                                          vehicle.DamageModel == null ||
                                          vehicle.DamageModel.BrokenWindows != BrokenWindows ||
                                          vehicle.DamageModel.BrokenDoors != BrokenDoors;

                            if (!syncUnocVeh) continue;
                            {
                                vehicle.Position = ent.Position;
                                vehicle.Rotation = ent.Rotation;
                                vehicle.Health = veh.Health;
                                vehicle.Tires = (byte)Util.Util.BuildTyreFlag(veh);

                                if (vehicle.DamageModel == null) vehicle.DamageModel = new VehicleDamageModel();

                                vehicle.DamageModel.BrokenWindows = BrokenWindows;
                                vehicle.DamageModel.BrokenDoors = BrokenDoors;

                                var data = new VehicleData
                                {
                                    VehicleHandle = vehicle.RemoteHandle,
                                    Position = vehicle.Position,
                                    Quaternion = vehicle.Rotation,
                                    Velocity = ent.Velocity,
                                    VehicleHealth = vehicle.Health,
                                    DamageModel = new VehicleDamageModel()
                                    {
                                        BrokenWindows = BrokenWindows,
                                        BrokenDoors = BrokenDoors,
                                    }
                                };

                                if (ent.IsDead)
                                {
                                    data.Flag = (short) VehicleDataFlags.VehicleDead;
                                }
                                else
                                {
                                    data.Flag = 0;
                                }

                                byte tyreFlag = 0;

                                for (int i = 0; i < 8; i++) if (veh.IsTireBurst(i)) tyreFlag |= (byte)(1 << i);

                                data.PlayerHealth = tyreFlag;

                                var bin = PacketOptimization.WriteUnOccupiedVehicleSync(data);

                                buffer.AddRange(bin);
                                vehicleCount++;
                            }
                        }
                    }

                    if (vehicleCount > 0)
                    {
                        buffer.Insert(0, (byte)vehicleCount);

                        var msg = Main.Client.CreateMessage();
                        msg.Write((byte)PacketType.UnoccupiedVehSync);
                        msg.Write(buffer.Count);
                        msg.Write(buffer.ToArray());

                        Main.Client.SendMessage(msg, NetDeliveryMethod.Unreliable, (int)ConnectionChannel.UnoccupiedVeh);

                        Main.BytesSent += buffer.Count;
                        Main.MessagesSent++;
                    }
                }
            }

            for (int i = 0; i < Interpolations.Count; i++)
            {
                var pair = Interpolations.ElementAt(i);

                pair.Value.Pulse();

                if (pair.Value.HasFinished) Interpolations.Remove(pair.Key);
            }
        }
    }

    internal class UnoccupiedVehicleInterpolator
    {
        internal int GameHandle;
        internal int NetHandle;
        private Vehicle _entity;
        private RemoteVehicle _prop;
        private Vector3 _velocity;
        private Vector3 _rotation;

        private Vector3 _startPos;
        private Vector3 _startRot;

        private NetInterpolation NetInterpolation;

        internal bool HasFinished;

        internal UnoccupiedVehicleInterpolator(int gameHandle, int netHandle)
        {
            GameHandle = gameHandle;
            NetHandle = netHandle;
            _entity = new Vehicle(gameHandle);
            _prop = Main.NetEntityHandler.NetToStreamedItem(netHandle) as RemoteVehicle;
        }

        internal void SetTargetPosition(Vector3 targetPos, Vector3 velocity, Vector3 rotation)
        {
            var dir = targetPos - _prop.Position;

            NetInterpolation.vecTarget = targetPos;
            NetInterpolation.vecError = dir;

            NetInterpolation.StartTime = Util.Util.TickCount;
            NetInterpolation.FinishTime = NetInterpolation.StartTime + 400;
            NetInterpolation.LastAlpha = 0f;

            _velocity = velocity;
            _rotation = rotation;

            _startPos = _prop.Position;
            _startRot = _prop.Rotation;

            _prop.Rotation = rotation;
            _prop.Position = targetPos;

            HasFinished = false;
        }

        internal void Pulse()
        {
            if (!HasFinished)
            {
                long currentTime = Util.Util.TickCount;
                float alpha = Util.Util.Unlerp(NetInterpolation.StartTime, currentTime, NetInterpolation.FinishTime);

                alpha = Util.Util.Clamp(0f, alpha, 1.5f);

                Vector3 comp = Util.Util.Lerp(new Vector3(), NetInterpolation.vecError, alpha);

                if (alpha == 1.5f)
                {
                    NetInterpolation.FinishTime = 0;
                    HasFinished = true;
                }

                var newPos = _startPos + comp;
                //_entity.Velocity = _velocity + 3 * (newPos - _entity.Position); IDK pourquoi j'ai une erreur qui n'est pas sur GTAN

                _entity.Quaternion = Quaternion.Slerp(_startRot.ToQuaternion(), _rotation.ToQuaternion(), Math.Min(1.5f, alpha));
            }
        }
    }

    struct NetInterpolation
    {
        internal Vector3 vecStart;
        internal Vector3 vecTarget;
        internal Vector3 vecError;
        internal long StartTime;
        internal long FinishTime;
        internal float LastAlpha;
    }
}