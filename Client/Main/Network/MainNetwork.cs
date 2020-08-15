using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using RDRN_Core;
using RDRN_Core.Native;
using RDRN_Core.Javascript;
using RDRN_Core.Misc;
using RDRN_Core.Streamer;
using RDRN_Core.Util;
using Shared;
using Lidgren.Network;
using RDRN_Core.Sync;
using RDRN_Core.Gui.Cef;

namespace RDRN_Core
{
    public partial class Main
    {
        public void ConnectToServer(string ip, int port = 0, bool passProtected = false, string myPass = "")
        {
            if (IsOnServer())
            {
                Client.Disconnect("Switching servers");
                //Wait(1000);
            }
            Function.Call(Hash.DISPLAY_RADAR, false);

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            if (!_minimapSet)
            {
                /*
                var scal = new Scaleform("minimap");
                scal.CallFunction("MULTIPLAYER_IS_ACTIVE", true, false);

                Function.Call(Hash._SET_RADAR_BIGMAP_ENABLED, true, false);
                Function.Call(Hash._SET_RADAR_BIGMAP_ENABLED, false, false);
                */
                _minimapSet = true;
            }

            Client.Shutdown("Shutdown");
           // Wait(1000);
            var cport = GetOpenUdpPort();
            if (cport == 0)
            {
                Util.Util.SafeNotify("No available UDP port was found.");
                return;
            }
            _config.Port = cport;
            Client = new NetClient(_config);
            Client.Start();

            lock (Npcs) Npcs = new Dictionary<string, SyncPed>();
            lock (_tickNatives) _tickNatives = new Dictionary<string, NativeData>();

            var msg = Client.CreateMessage();

            var obj = new ConnectionRequest();
            obj.SocialClubName = string.IsNullOrWhiteSpace(Game.Player.Name) ? "Unknown" : Game.Player.Name; // To be used as identifiers in server files
            obj.DisplayName = string.IsNullOrWhiteSpace(PlayerSettings.DisplayName) ? obj.SocialClubName : PlayerSettings.DisplayName.Trim();
            obj.ScriptVersion = CurrentVersion.ToString();
            obj.CEFDevtool = EnableDevTool;
            //obj.GameVersion = (byte)Game.Version;
            obj.MediaStream = EnableMediaStream;

            if (passProtected)
            {
                if (!string.IsNullOrWhiteSpace(myPass))
                {
                    obj.Password = myPass;
                }
                else
                {
                    //MainMenu.TemporarilyHidden = true;
                    obj.Password = Game.GetUserInput(24);
                   // MainMenu.TemporarilyHidden = false;
                }
            }

            var bin = SerializeBinary(obj);

            msg.Write((byte)PacketType.ConnectionRequest);
            msg.Write(bin.Length);
            msg.Write(bin);

            try
            {
                Client.Connect(ip, port == 0 ? Port : port, msg);
            }
            catch (NetException ex)
            {
                //RDR2.UI.Screen.ShowNotification("~b~~h~GTA Network~h~~w~~n~" + ex.Message);
                OnLocalDisconnect();
                return;
            }

            var pos = Game.Player.Character.Position;
            /*
            Function.Call(Hash.CLEAR_AREA_OF_PEDS, pos.X, pos.Y, pos.Z, 100f, 0);
            Function.Call(Hash.CLEAR_AREA_OF_VEHICLES, pos.X, pos.Y, pos.Z, 100f, 0);

            Function.Call(Hash.SET_GARBAGE_TRUCKS, 0);*/
            Function.Call(Hash.SET_RANDOM_BOATS, 0);
            Function.Call(Hash.SET_RANDOM_TRAINS, 0);

            //Function.Call(Hash.CLEAR_ALL_BROKEN_GLASS);

            //DisableSlowMo();

            Game.TimeScale = 1;

            ResetPlayer();

            _currentServerIp = ip;
            _currentServerPort = port == 0 ? Port : port;
        }

        public static bool IsOnServer()
        {
            return Client?.ConnectionStatus == NetConnectionStatus.Connected;
        }

        private void OnLocalDisconnect()
        {
            DEBUG_STEP = 42;
            if (NetEntityHandler.ServerWorld?.LoadedIpl != null)
            {/*
                foreach (var ipl in NetEntityHandler.ServerWorld.LoadedIpl)
                    Function.Call(Hash.REMOVE_IPL, ipl);*/
            }

            DEBUG_STEP = 43;
            if (NetEntityHandler.ServerWorld?.RemovedIpl != null)
            {
                /*
                foreach (var ipl in NetEntityHandler.ServerWorld.RemovedIpl)
                {
                    Function.Call(Hash.REQUEST_IPL, ipl);
                }*/
            }

            DEBUG_STEP = 44;

            ClearLocalEntities();

            DEBUG_STEP = 45;

            ClearLocalBlips();

            DEBUG_STEP = 49;
            NetEntityHandler.ClearAll();
            DEBUG_STEP = 50;
            JavascriptHook.StopAllScripts();
            //JavascriptHook.TextElements.Clear();
            SyncCollector.ForceAimData = false;
            StringCache.Dispose();
            StringCache = null;
            _threadsafeSubtitle = null;
            _cancelDownload = true;
            _httpDownloadThread?.Abort();
            CefController.ShowCursor = false;
            DEBUG_STEP = 51;
            DownloadManager.Cancel();
            DownloadManager.FileIntegrity.Clear();
            WeaponInventoryManager.Clear();
            VehicleSyncManager.StopAll();
            HasFinishedDownloading = false;
            ScriptChatVisible = true;
            CanOpenChatbox = true;
            DisplayWastedMessage = true;
            _password = string.Empty;
            
            //CEFManager.Draw = false;


            UIColor = Color.White;

            DEBUG_STEP = 52;

            lock (CEFManager.Browsers)
            {
                foreach (var browser in CEFManager.Browsers)
                {
                    browser.Close();
                    browser.Dispose();
                }

                CEFManager.Browsers.Clear();
            }

            CEFManager.Dispose();
            ClearStats();

            RestoreMainMenu();

            DEBUG_STEP = 56;

            ResetWorld();

            DEBUG_STEP = 57;

            ResetPlayer();

            DEBUG_STEP = 58;

            if (_serverProcess != null)
            {
                //RDR2.UI.Screen.ShowNotification("~b~~h~GTA Network~h~~w~~n~Shutting down server...");
                _serverProcess.Kill();
                _serverProcess.Dispose();
                _serverProcess = null;
            }
        }

        public static void SendToServer(object newData, PacketType packetType, bool important, ConnectionChannel channel)
        {
            var data = SerializeBinary(newData);
            NetOutgoingMessage msg = Client.CreateMessage();
            msg.Write((byte)packetType);
            msg.Write(data.Length);
            msg.Write(data);
            Client.SendMessage(msg, important ? NetDeliveryMethod.ReliableOrdered : NetDeliveryMethod.ReliableSequenced, (int)channel);
        }

        public static void TriggerServerEvent(string eventName, string resource, params object[] args)
        {
            if (!IsOnServer()) return;
            var packet = new ScriptEventTrigger();
            packet.EventName = eventName;
            packet.Resource = resource;
            packet.Arguments = ParseNativeArguments(args);
            var bin = SerializeBinary(packet);

            var msg = Client.CreateMessage();
            msg.Write((byte)PacketType.ScriptEventTrigger);
            msg.Write(bin.Length);
            msg.Write(bin);

            Client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }


        public int GetOpenUdpPort()
        {


            var startingAtPort = 49152;
            var maxNumberOfPortsToCheck = 65535;
            var range = Enumerable.Range(startingAtPort, maxNumberOfPortsToCheck);
            var enumerable = range as IList<int> ?? range.ToList();
            var portsInUse =
                from p in enumerable
                join used in System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()
            on p equals used.Port
                select p;

            var inUse = portsInUse as IList<int> ?? portsInUse.ToList();

            Random rand = new Random();
            int toTake = rand.Next(0, inUse.Count);


            return enumerable.Except(inUse).ElementAtOrDefault(toTake);
        }

        public static void HandleUnoccupiedVehicleSync(VehicleData data)
        {
            if (data.VehicleHandle != null)
            {
                var car = NetEntityHandler.NetToStreamedItem(data.VehicleHandle.Value) as RemoteVehicle;

                if (car != null)
                {
                    if (data.VehicleHealth != null) car.Health = data.VehicleHealth.Value;
                    car.IsDead = (data.Flag & (int)VehicleDataFlags.VehicleDead) != 0;

                    if (car.DamageModel == null) car.DamageModel = new VehicleDamageModel();
                    car.DamageModel.BrokenWindows = data.DamageModel.BrokenWindows;
                    car.DamageModel.BrokenDoors = data.DamageModel.BrokenDoors;

                    if (data.PlayerHealth != null)
                    {
                        car.Tires = data.PlayerHealth.Value;

                        if (car.StreamedIn)
                        {
                            var ent = NetEntityHandler.NetToEntity(data.VehicleHandle.Value);

                            if (ent != null)
                            {
                                if (data.Velocity != null)
                                {
                                    VehicleSyncManager.Interpolate(data.VehicleHandle.Value, ent.Handle, data.Position, data.Velocity, data.Quaternion);
                                }
                                else
                                {
                                    car.Position = data.Position;
                                    car.Rotation = data.Quaternion;
                                }

                                var veh = new Vehicle(ent.Handle);

                                //veh.SetVehicleDamageModel(car.DamageModel);

                                //veh.EngineHealth = car.Health;
                                if (!ent.IsDead && car.IsDead)
                                {
                                    ent.IsInvincible = false;
                                    //veh.Explode();
                                }

                                for (int i = 0; i < 8; i++)
                                {
                                    bool busted = (data.PlayerHealth.Value & (byte)(1 << i)) != 0;
                                    /*
                                    if (busted && !veh.IsTireBurst(i)) veh.Wheels[i].Burst();
                                    else if (!busted && veh.IsTireBurst(i)) veh.Wheels[i].Fix();*/
                                }
                            }
                        }
                        else
                        {
                            car.Position = data.Position;
                            car.Rotation = data.Quaternion;
                        }
                    }
                }
            }
        }

        private static bool isIPLocal(string ipaddress)
        {
            try
            {
                var straryIpAddress = ipaddress.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                var iaryIpAddress = new[]
                {
                    int.Parse(straryIpAddress[0], CultureInfo.InvariantCulture),
                    int.Parse(straryIpAddress[1], CultureInfo.InvariantCulture),
                    int.Parse(straryIpAddress[2], CultureInfo.InvariantCulture),
                    int.Parse(straryIpAddress[3], CultureInfo.InvariantCulture)
                };


                return iaryIpAddress[0] == 10 || iaryIpAddress[0] == 127 || iaryIpAddress[0] == 192 && iaryIpAddress[1] == 168 || iaryIpAddress[0] == 172 && iaryIpAddress[1] >= 16 && iaryIpAddress[1] <= 31;

                // IP Address is "probably" public. This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            }
            catch
            {
                return false;
            }
        }

    }
}
