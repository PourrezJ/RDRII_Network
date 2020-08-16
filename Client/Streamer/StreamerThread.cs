﻿using System;
using System.Collections.Generic;
using System.Linq;
using RDRN_Core;
using RDRN_Core.Util;
using RDRN_Core.Sync;
using Vector3 = Shared.Math.Vector3;
using System.Diagnostics;

namespace RDRN_Core.Streamer
{
    internal class StreamerThread : Script
    {
        public static SyncPed[] SyncPeds;
        public static SyncPed[] StreamedInPlayers;
        public static RemoteVehicle[] StreamedInVehicles;

        private static List<IStreamedItem> _itemsToStreamIn;
        private static List<IStreamedItem> _itemsToStreamOut;

        public static Stopwatch sw;

        public StreamerThread()
        {
            _itemsToStreamIn = new List<IStreamedItem>();
            _itemsToStreamOut = new List<IStreamedItem>();
            StreamedInPlayers = new SyncPed[MAX_PLAYERS];

            Tick += StreamerTick;

            var calcucationThread = new System.Threading.Thread(StreamerCalculationsThread) { IsBackground = true };
            calcucationThread.Start();
        }

        private static Vector3 _playerPosition;

        public const int MAX_PLAYERS = 250; //Max engine ped value: 256, on 236 it starts to cause issues
        public const int MAX_OBJECTS = 500; //Max engine value: 2500
        public const int MAX_VEHICLES = 60; //Max engine value: 64 +/ 1
        public const int MAX_PICKUPS = 50; //NEEDS A TEST
        public const int MAX_BLIPS = 50; //Max engine value: 1298
        public static int MAX_PEDS; //Share the Ped limit, prioritize the players
        public const int MAX_LABELS = MAX_PLAYERS; //NEEDS A TEST
        public const int MAX_MARKERS = 120; //Max engine value: 128
        public const int MAX_PARTICLES = 50;

        private const float GlobalRange = 2000f;
        private const float MediumRange = 1000f;
        private const float CloseRange = 500f;

        private const float GlobalRangeSquared = GlobalRange * GlobalRange;
        private const float MediumRangeSquared = MediumRange * MediumRange;
        private const float CloseRangeSquared = CloseRange * CloseRange;


        private static void StreamerCalculationsThread()
        {
            while (true)
            {
                if (!Main.IsOnServer || !Main.HasFinishedDownloading) goto endTick;
                var position = _playerPosition;

                IStreamedItem[] rawMap;
                lock (Main.NetEntityHandler.ClientMap) rawMap = Main.NetEntityHandler.ClientMap.Values.Where(item => !(item is RemotePlayer) || ((RemotePlayer) item).LocalHandle != -2).ToArray();
                
                #region Players
                SyncPeds = rawMap.OfType<SyncPed>().ToArray();
                var streamedInPlayers = SyncPeds.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn)
                {
                    _itemsToStreamIn.AddRange(streamedInPlayers.Take(MAX_PLAYERS).Where(item => !item.StreamedIn));
                }
                lock (StreamedInPlayers)
                {
                    StreamedInPlayers = streamedInPlayers.Take(MAX_PLAYERS).ToArray();
                }


                var streamedOutPlayers = SyncPeds.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0 || !IsInRangeSquared(position, item.Position, GlobalRangeSquared)) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInPlayers.Skip(MAX_PLAYERS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutPlayers);
                }
                #endregion

                var entityMap = rawMap.Where(item => item.Position != null).ToArray();

                #region Vehicles
                var Vehicles = entityMap.OfType<RemoteVehicle>().ToArray();

                StreamedInVehicles = Vehicles.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn)
                {
                    _itemsToStreamIn.AddRange(StreamedInVehicles.Take(MAX_VEHICLES).Where(item => !item.StreamedIn));
                }

                var streamedOutVehicles = Vehicles.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0) || !IsInRangeSquared(position, item.Position, GlobalRangeSquared) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(StreamedInVehicles.Skip(MAX_VEHICLES).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutVehicles);
                }
                #endregion

                #region Objects
                var Objects = entityMap.OfType<RemoteProp>().ToArray();

                var streamedInObjects = Objects.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn)
                {
                    _itemsToStreamIn.AddRange(streamedInObjects.Take(MAX_OBJECTS).Where(item => !item.StreamedIn));
                }

                var streamedOutObjects = Objects.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0 || !IsInRangeSquared(position, item.Position, GlobalRangeSquared)) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInObjects.Skip(MAX_OBJECTS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutObjects);
                }

                #endregion

                #region Other Shit
                var Markers = entityMap.OfType<RemoteMarker>().ToArray();
                var streamedInMarkers = Markers.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInMarkers.Take(MAX_MARKERS).Where(item => !item.StreamedIn));

                var streamedOutMarkers = Markers.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0) || !IsInRangeSquared(position, item.Position, GlobalRangeSquared) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInMarkers.Skip(MAX_MARKERS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutMarkers);
                }


                var Peds = entityMap.OfType<RemotePed>().ToArray();
                MAX_PEDS = MAX_PLAYERS - streamedInPlayers.Take(MAX_PLAYERS).Count();
                var streamedInPeds = Peds.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInPeds.Take(MAX_PEDS).Where(item => !item.StreamedIn));

                var streamedOutPeds = Peds.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0 || !IsInRangeSquared(position, item.Position, GlobalRangeSquared)) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInPeds.Skip(MAX_PEDS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutPeds);
                }


                var Labels = entityMap.OfType<RemoteTextLabel>().ToArray();
                var streamedInLabels = Labels.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, CloseRangeSquared)).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInLabels.Take(MAX_LABELS).Where(item => !item.StreamedIn));

                var streamedOutLabels = Labels.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0 || !IsInRangeSquared(position, item.Position, CloseRangeSquared)) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInLabels.Skip(MAX_LABELS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutLabels);
                }


                var Particles = entityMap.OfType<RemoteParticle>().ToArray();
                var streamedInParticles = Particles.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, GlobalRangeSquared)).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInParticles.Take(MAX_PARTICLES).Where(item => !item.StreamedIn));

                var streamedOutParticles = Particles.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0) || !IsInRangeSquared(position, item.Position, GlobalRangeSquared) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInParticles.Skip(MAX_PARTICLES).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutParticles);
                }


                var Pickups = entityMap.OfType<RemotePickup>().ToArray();
                var streamedInPickups = Pickups.Where(item => (item.Dimension == Main.LocalDimension || item.Dimension == 0) && IsInRangeSquared(position, item.Position, MediumRangeSquared)).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInPickups.Take(MAX_PICKUPS).Where(item => !item.StreamedIn));

                var streamedOutPickups = Pickups.Where(item => (item.Dimension != Main.LocalDimension && item.Dimension != 0) || !IsInRangeSquared(position, item.Position, MediumRangeSquared) && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInPickups.Skip(MAX_PICKUPS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutPickups);
                }


                var Blips = entityMap.OfType<RemoteBlip>().ToArray();
                var streamedInBlips = Blips.Where(item => item.Dimension == Main.LocalDimension || item.Dimension == 0).ToArray();
                lock (_itemsToStreamIn) _itemsToStreamIn.AddRange(streamedInBlips.Take(MAX_BLIPS).Where(item => !item.StreamedIn));

                var streamedOutBlips = Blips.Where(item => item.Dimension != Main.LocalDimension && item.Dimension != 0 && item.StreamedIn);
                lock (_itemsToStreamOut)
                {
                    _itemsToStreamOut.AddRange(streamedInBlips.Skip(MAX_BLIPS).Where(item => item.StreamedIn));
                    _itemsToStreamOut.AddRange(streamedOutBlips);
                }
                #endregion

                endTick:
                System.Threading.Thread.Sleep(500);
            }
        }

        private static void StreamerTick(object sender, EventArgs e)
        {
            _playerPosition = Game.Player.Character.Position;
            if (Util.Util.ModelRequest) return;
            sw = new Stopwatch();

            lock (_itemsToStreamOut)
            {
                var count = _itemsToStreamOut.Count;
                for (var i = 0; i < count; i++)
                {
                    Main.NetEntityHandler.StreamOut(_itemsToStreamOut[i]);
                }
                _itemsToStreamOut.Clear();
            }

            lock (_itemsToStreamIn)
            {
                var count = _itemsToStreamIn.Count;
                for (var i = 0; i < count; i++)
                {
                    Main.NetEntityHandler.StreamIn(_itemsToStreamIn[i]);
                }
                _itemsToStreamIn.Clear();
            }
        }

        private static bool IsInRange(Vector3 center, Vector3 dest, float range)
        {
            return center.Subtract(dest).Length() <= range;
        }

        private static bool IsInRangeSquared(Vector3 center, Vector3 dest, float range)
        {
            return center.Subtract(dest).LengthSquared() <= range;
        }
    }

}