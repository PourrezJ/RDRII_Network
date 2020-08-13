using RDRN_Core;
using RDRN_Core.Native;
using RDRN_Core.Misc;
using RDRN_Core.Streamer;
using RDRN_Core.Sync;
using RDRN_Core.Util;
using Shared;
using Lidgren.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Vector3 = Shared.Math.Vector3;
using RDRN_Core.Gui.Cef;
using RDRN_Core.Gui.DirectXHook;

namespace RDRN_Core
{
    internal class MessagePump : Script
    {
        public MessagePump()
        {
            Tick += (sender, args) =>
            {
                if (Main.Client != null)
                {
                    var messages = new List<NetIncomingMessage>();
                    var msgsRead = Main.Client.ReadMessages(messages);
                    if (msgsRead > 0)
                    {
                        var count = messages.Count;
                        for (var i = 0; i < count; i++)
                        {
                            CrossReference.EntryPoint.ProcessMessages(messages[i], true);
                        }
                    }
                }
            };
        }
    }

    internal static class CrossReference
    {
        public static Main EntryPoint;
    }

    public partial class Main : Script
    {
        #region garbage
        public static PlayerSettings PlayerSettings;

        public static Size screen;

        public static readonly ScriptVersion LocalScriptVersion = ScriptVersion.VERSION_0_9;
        public static readonly string build = "exp";

        public static bool BlockControls;
        public static bool HTTPFileServer;

        public static bool IsSpectating;
        private static Vector3 _preSpectatorPos;

        internal static Streamer.Streamer NetEntityHandler;

        public static SizeF res;


        private string _clientIp;

        public static NetClient Client;
        private static NetPeerConfiguration _config;
        public static ParseableVersion CurrentVersion = ParseableVersion.FromAssembly(Assembly.GetExecutingAssembly());

        public static bool LerpRotaion = true;

        public static bool _wasTyping;

        public static bool RemoveGameEntities = true;
        public static bool ChatVisible = true;
        public static bool CanOpenChatbox = true;
        public static bool DisplayWastedMessage = true;
        public static bool ScriptChatVisible = true;
        public static bool UIVisible = true;
        public static Color UIColor = Color.White;

        public static StringCache StringCache;

        public static int LocalTeam = -1;
        public static int LocalDimension = 0;
        public int SpectatingEntity;

        private static readonly Queue<Action> _threadJumping = new Queue<Action>();
        private string _password;
        private string _QCpassword;

        private static SyncEventWatcher Watcher;
        internal static UnoccupiedVehicleSync VehicleSyncManager;
        internal WeaponManager WeaponInventoryManager;



        // STATS


        private static bool EnableDevTool;
        internal static bool EnableMediaStream;
        internal static bool SaveDebugToFile = false;

        public static bool ToggleNametagDraw = false;
        public static bool TogglePosUpdate = false;
        public static bool SlowDownClientForDebug = false;
        internal static bool _playerGodMode;


        public static RelationshipGroup RelGroup;
        public static RelationshipGroup FriendRelGroup;
        public static bool HasFinishedDownloading;
        public static string SocialClubName;

        #region Debug stuff
        private bool display;
        private Ped mainPed;
        private Vehicle mainVehicle;

        private Vector3 oldplayerpos;
        private bool _lastJumping;
        private bool _lastShooting;
        private bool _lastAiming;
        private uint _switch;
        private bool _lastVehicle;
        private bool _oldChat;
        private bool _isGoingToCar;
        #endregion

        public static bool JustJoinedServer { get; set; }


        private static Process _serverProcess;

        private static int _currentServerPort;
        private static string _currentServerIp;

        internal static Dictionary<string, SyncPed> Npcs;
        internal static float Latency;
        private static int Port = 4499;

        private static GameSettings.Settings GameSettings;

        private static string CustomAnimation;
        private static int AnimationFlag;

        public static Camera MainMenuCamera;

        private static int _debugStep;
        private static Dictionary<int, int> _debugSettings = new Dictionary<int, int>();
        private static bool _minimapSet;

        internal static string _threadsafeSubtitle;

        private static Dictionary<int, int> _emptyVehicleMods;
        private static Dictionary<string, NativeData> _tickNatives;
        private static Dictionary<string, NativeData> _dcNatives;

        public static List<int> EntityCleanup;
        public static List<int> BlipCleanup;
        public static Dictionary<int, MarkerProperties> _localMarkers = new Dictionary<int, MarkerProperties>();

        private static Dictionary<int, int> _vehMods = new Dictionary<int, int>();
        private static Dictionary<int, int> _pedClothes = new Dictionary<int, int>();

        public static string Weather { get; set; }
        public static TimeSpan? Time { get; set; }


        private static bool _init = false;

        public const NetDeliveryMethod SYNC_MESSAGE_TYPE = NetDeliveryMethod.UnreliableSequenced; // unreliable_sequenced

        #endregion


        internal static Size Screen
        {
            get
            {
                int w, h;
                unsafe { Function.Call(Hash.GET_SCREEN_RESOLUTION, &w, &h); }
                return new Size(w, h);
            }
        }

        internal static DxHook DxHook { get; private set; }

        private static bool firstTick;

        public Main()
        {
            //res = UIMenu.GetScreenResolutionMantainRatio();
            screen = RDRN_Core.UI.Screen.Resolution;

            LogManager.WriteLog("\r\n>> [" + DateTime.Now + "] RDR2 Network Initialization.");

            World.DestroyAllCameras();

            CrossReference.EntryPoint = this;

            GameSettings = Misc.GameSettings.LoadGameSettings();
            PlayerSettings = Util.Util.ReadSettings(RDRNetworkPath + "\\settings.xml");

            EnableDevTool = PlayerSettings.CEFDevtool;

            NetEntityHandler = new Streamer.Streamer();

            Watcher = new SyncEventWatcher(this);
            VehicleSyncManager = new UnoccupiedVehicleSync();
            WeaponInventoryManager = new WeaponManager();

            Npcs = new Dictionary<string, SyncPed>();
            _tickNatives = new Dictionary<string, NativeData>();
            _dcNatives = new Dictionary<string, NativeData>();

            EntityCleanup = new List<int>();
            BlipCleanup = new List<int>();

            _emptyVehicleMods = new Dictionary<int, int>();
            for (var i = 0; i < 50; i++) {
                _emptyVehicleMods.Add(i, 0);
            }
            /*
            KeyDown += OnKeyDown;

            KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape && _wasTyping)
                {
                    _wasTyping = false;
                }
            };*/

            _config = new NetPeerConfiguration("GTANETWORK") { Port = 8888, ConnectionTimeout = 30f };
            _config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            LogManager.WriteLog("Building menu.");
            /*
            _menuPool = new MenuPool();
            BuildMainMenu();

            Function.Call(Hash._ENABLE_MP_DLC_MAPS, true); // _ENABLE_MP_DLC_MAPS
            Function.Call(Hash._LOAD_MP_DLC_MAPS); // _LOAD_MP_DLC_MAPS / _USE_FREEMODE_MAP_BEHAVIOR
            */
            MainMenuCamera = World.CreateCamera(new Vector3(743.76f, 1070.7f, 350.24f), new Vector3(), GameplayCamera.FieldOfView);
            MainMenuCamera.PointAt(new Vector3(707.86f, 1228.09f, 333.66f));

            RelGroup = World.AddRelationshipGroup("SYNCPED");
            FriendRelGroup = World.AddRelationshipGroup("SYNCPED_TEAMMATES");

            RelGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, (Relationship)255, true);
            FriendRelGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, Relationship.Companion, true);

            SocialClubName = Game.Player.Name;

            LogManager.WriteLog("Getting welcome message.");
           // GetWelcomeMessage();

            //Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);

            //Audio.SetAudioFlag(AudioFlag.LoadMPData, true);
            //Audio.SetAudioFlag(AudioFlag.DisableBarks, true);
            //Audio.SetAudioFlag(AudioFlag.DisableFlightMusic, true);
            //Audio.SetAudioFlag(AudioFlag.PoliceScannerDisabled, true);
            //Audio.SetAudioFlag(AudioFlag.OnlyAllowScriptTriggerPoliceScanner, true);
            //Function.Call((Hash)0x552369F549563AD5, false); //_FORCE_AMBIENT_SIREN

            // disable fire dep dispatch service
            //Function.Call((Hash)0xDC0F817884CDD856, 4, false); // ENABLE_DISPATCH_SERVICE

            //GlobalVariable.Get(2576573).Write(1); //Enable MP cars?

            //var fetchThread = new Thread((ThreadStart) delegate
            //{
            //    var list = Process.GetProcessesByName("GameOverlayUI");
            //    if (!list.Any()) return;
            //    for (var index = list.Length - 1; index >= 0; index--) list[index].Kill();
            //});

            //fetchThread.Start();

            LogManager.WriteLog("Initializing CEF.");
            CEFManager.InitializeCef();

            LogManager.WriteLog("Rebuilding Server Browser.");
            RebuildServerBrowser();

            LogManager.WriteLog("Checking game files integrity.");
        }

        private static void Init()
        {
            if (_init) 
                return;

            var player = Game.Player.Character;
            if (player == null || player.Handle == 0 || Game.IsLoading) 
                return;

            _init = true;

            RDRN_Core.UI.Screen.FadeOut(1);
            ResetPlayer();
            //MainMenu.RefreshIndex();
           
            //MainMenu.Visible = true;
            World.RenderingCamera = MainMenuCamera;

            Game.TimeScale = 1;

            RDRN_Core.UI.Screen.FadeIn(1000);
            //_mainWarning = new Warning("",""){ Visible = false};
        }

        public static bool IsConnected()
        {
            if (Client == null)
                return false;

            var status = Client.ConnectionStatus;

            return status != NetConnectionStatus.Disconnected && status != NetConnectionStatus.None;
        }

        public static void OnTick()
        {
            Init();

            PauseMenu();

            if (!IsConnected()) return;

            if (DateTime.Now.Subtract(_lastCheck).TotalMilliseconds > 1000)
            {
                _bytesSentPerSecond = BytesSent - _lastBytesSent;
                _bytesReceivedPerSecond = BytesReceived - _lastBytesReceived;

                _lastBytesReceived = BytesReceived;
                _lastBytesSent = BytesSent;

                _lastCheck = DateTime.Now;
            }

            try
            {
                DEBUG_STEP = 1;
                Watcher?.Tick();

                DEBUG_STEP = 2;
                VehicleSyncManager?.Pulse();

                DEBUG_STEP = 3;
                WeaponManager.Update();
            }
            catch (Exception ex) // Catch any other exception. (can prevent crash)
            {
                LogManager.Exception(ex, "MAIN OnTick: STEP : " + DEBUG_STEP);
            }

            //Spectate(res);

            //if (NetEntityHandler.EntityToStreamedItem(PlayerChar.Handle) is RemotePlayer playerObj) Game.Player.IsInvincible = playerObj.IsInvincible;

            //var playerChar = FrameworkData.PlayerChar.Ex();
            //if (!string.IsNullOrWhiteSpace(CustomAnimation))
            //{
            //    var sp = CustomAnimation.Split();
            //    if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, playerChar, sp[0], sp[1], 3))
            //    {
            //        playerChar.Task.ClearSecondary();
            //        Function.Call(Hash.TASK_PLAY_ANIM, playerChar, Util.Util.LoadDict(sp[0]), sp[1], 8f, 10f, -1, AnimationFlag, -8f, 1, 1, 1);
            //    }
            //}

            StringCache?.Pulse();
            lock (_threadJumping)
            {
                if (_threadJumping.Any())
                {
                    var action = _threadJumping.Dequeue();
                    action?.Invoke();
                }
            }

        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                if (Client != null && IsOnServer()) Client.Disconnect("Quit");
                CEFManager.Dispose();
                CEFManager.DisposeCef();

                Process.GetProcessesByName("RDR2")[0].Kill();
                Process.GetCurrentProcess().Kill();
            }
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (IsOnServer())
                    {
                        if (_isGoingToCar)
                        {
                            Game.Player.Character.Task.ClearAll();
                            _isGoingToCar = false;
                        }
                    }
                    if (Client != null && Client.ConnectionStatus == NetConnectionStatus.Disconnected)
                    {
                        Client.Disconnect("Quit");
                    }
                    break;

                case Keys.P:
                    if (IsOnServer() && /*!MainMenu.Visible &&*/ !CefController.ShowCursor)
                    {
                        /*
                        _mainWarning = new Warning("Disabled feature", "Game settings menu has been disabled while connected.\nDisconnect from the server first.")
                        {
                            OnAccept = () => { _mainWarning.Visible = false; }
                        };*/
                    }
                    break;

                case Keys.F10:

                    break;

                case Keys.F7:
                    if (IsOnServer())
                    {
                        ChatVisible = !ChatVisible;
                        UIVisible = !UIVisible;
                        Function.Call(Hash.DISPLAY_RADAR, UIVisible);
                        Function.Call(Hash.DISPLAY_HUD, UIVisible);
                    }
                    break;

                case Keys.T:
                    if (IsOnServer() && UIVisible && ChatVisible && ScriptChatVisible && CanOpenChatbox)
                    {
                        if (!_oldChat)
                        {
                            _wasTyping = true;
                        }
                        else
                        {
                            var message = Game.GetUserInput(99);
                            if (string.IsNullOrEmpty(message)) break;

                            var obj = new ChatData { Message = message, };
                            var data = SerializeBinary(obj);

                            var msg = Client?.CreateMessage();
                            msg?.Write((byte)PacketType.ChatData);
                            msg?.Write(data.Length);
                            msg?.Write(data);
                            Client?.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, (int)ConnectionChannel.SyncEvent);
                        }
                    }
                    break;

                case Keys.G:
                    /*
                    if (IsOnServer() && !Game.Player.Character.IsInVehicle() && !Chat.IsFocused)
                    {
                        //var veh = new Vehicle(StreamerThread.StreamedInVehicles[0].LocalHandle);
                        List<Vehicle> vehs;
                        if (!(vehs = World.GetAllVehicles().OrderBy(v => v.Position.DistanceToSquared(Game.Player.Character.Position)).Take(1).ToList()).Any()) break;

                        Vehicle veh;
                        if (!(veh = vehs[0]).Exists()) break;
                        if (!Game.Player.Character.IsInRangeOfEx(veh.Position, 6f)) break;

                        var playerChar = Game.Player.Character;
                        var relPos = veh.GetOffsetFromWorldCoords(playerChar.Position);
                        var seat = VehicleSeat.Any;

                        if (veh.PassengerCapacity > 1)
                        {
                            if (relPos.X < 0 && relPos.Y > 0) {seat = VehicleSeat.LeftRear;}
                            else if (relPos.X >= 0 && relPos.Y > 0) {seat = VehicleSeat.RightFront;}
                            else if (relPos.X < 0 && relPos.Y <= 0) {seat = VehicleSeat.LeftRear;}
                            else if (relPos.X >= 0 && relPos.Y <= 0) {seat = VehicleSeat.RightRear;}
                        }
                        else { seat = VehicleSeat.Passenger; }

                        //else if (veh.PassengerCapacity > 2 && veh.GetPedOnSeat(seat).Handle != 0)
                        //{
                        //    switch (seat)
                        //    {
                        //        case VehicleSeat.LeftRear:
                        //            for (int i = 3; i < veh.PassengerCapacity; i += 2)
                        //            {
                        //                if (veh.GetPedOnSeat((VehicleSeat) i).Handle != 0) continue;
                        //                seat = (VehicleSeat) i;
                        //                break;
                        //            }
                        //            break;
                        //        case VehicleSeat.RightRear:
                        //            for (int i = 4; i < veh.PassengerCapacity; i += 2)
                        //            {
                        //                if (veh.GetPedOnSeat((VehicleSeat) i).Handle != 0) continue;
                        //                seat = (VehicleSeat) i;
                        //                break;
                        //            }
                        //            break;
                        //    }
                        //}

                        if (WeaponDataProvider.DoesVehicleSeatHaveGunPosition((VehicleHash)veh.Model.Hash, 0, true) && playerChar.IsIdle && !Game.Player.IsAiming)
                        {
                            playerChar.SetIntoVehicle(veh, seat);
                        }
                        else
                        {
                            veh.GetPedOnSeat(seat).CanBeDraggedOutOfVehicle = true;
                            playerChar.Task.EnterVehicle(veh, seat, -1, 2f);
                            Function.Call(Hash.KNOCK_PED_OFF_VEHICLE, veh.GetPedOnSeat(seat).Handle);
                            Function.Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, veh.GetPedOnSeat(seat).Handle, true); // 7A6535691B477C48 8A251612
                            _isGoingToCar = true;
                        }
                    }*/
                    break;

                default:
                    break;
            }
        }

    }
}
