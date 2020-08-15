using RDRN_Core.Native;
using RDRN_Core.Misc;
using RDRN_Core.Streamer;
using RDRN_Core.Sync;
using RDRN_Core.Util;
using Shared;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Vector3 = Shared.Math.Vector3;
using RDRN_Core.Gui.Cef;
using RDRN_Core.Gui.DirectXHook;

namespace RDRN_Core
{

    internal static class CrossReference
    {
        public static Main EntryPoint;
    }

    public partial class Main : Script
    {
        #region garbage
        public static PlayerSettings PlayerSettings;

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

        public static Camera MainMenuCamera;

        private static int _debugStep;

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

        public static int AnimationFlag { get; private set; }
        public static string CustomAnimation { get; private set; }

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

        public Main()
        {
            //res = UIMenu.GetScreenResolutionMantainRatio();

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
            
            KeyDown += OnKeyDown;

            KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape && _wasTyping)
                {
                    _wasTyping = false;
                }
            };

            Tick += OnTick;

            _config = new NetPeerConfiguration("RDRNETWORK") { Port = GetOpenUdpPort(), ConnectionTimeout = 30f };
            _config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            MainMenuCamera = World.CreateCamera(new Vector3(743.76f, 1070.7f, 350.24f), new Vector3(), GameplayCamera.FieldOfView);
            MainMenuCamera.PointAt(new Vector3(707.86f, 1228.09f, 333.66f));

            RelGroup = World.AddRelationshipGroup("SYNCPED");
            FriendRelGroup = World.AddRelationshipGroup("SYNCPED_TEAMMATES");

            RelGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, (Relationship)255, true);
            FriendRelGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, Relationship.Companion, true);

            SocialClubName = Game.Player.Name;

            LogManager.WriteLog("Getting welcome message.");

            LogManager.WriteLog("Initializing CEF.");
            CEFManager.InitializeCef();

            LogManager.WriteLog("Rebuilding Server Browser.");
            RebuildServerBrowser();

            LogManager.WriteLog("Checking game files integrity.");
        }

        private void Init()
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

            UI.Hud.IsRadarVisible = false;
           
            RDRN_Core.UI.Screen.FadeIn(1000);
            //_mainWarning = new Warning("",""){ Visible = false};

            CrossReference.EntryPoint.ConnectToServer("127.0.0.1", 4499);

        }

        public static bool IsConnected()
        {
            if (Client == null)
                return false;

            var status = Client.ConnectionStatus;

            return status != NetConnectionStatus.Disconnected && status != NetConnectionStatus.None;
        }

        public void OnTick(object sender, EventArgs e)
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

                    break;

                default:
                    break;
            }
        }

    }
}
