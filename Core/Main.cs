using RDRN_API;
using RDRN_API.Native;
using RDRN_Core.Gui.Cef;
using RDRN_Module;
using RDRN_Module.Native;
using RDRN_Core.Gui.DirectXHook;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharpDX.DXGI;

namespace RDRN_Core
{
    public partial class Main : Script
    {
        internal static bool InStartMenu;

        internal static string RDRNetworkPath
        {
            get => System.IO.Directory.GetParent(ScriptDomain.CurrentDir).FullName;
        }

        internal static Size Screen
        {
            get
            {
                int w, h;
                unsafe { Function.Call(Hash.GET_SCREEN_RESOLUTION, &w, &h); }
                return new Size(w, h);
            }
        }

        internal static PointF MousePos;

        internal static DxHook DxHook { get; private set; }

        public override void OnInit()
        {
            LogManager.WriteLog("Core Initializing");

            LogManager.WriteLog("PrepareNetwork configuration");
            PrepareNetwork();

            LogManager.WriteLog("DirectX hook Initializing");
            DxHook = new DxHook();

            LogManager.WriteLog("Cef Initializing");
            CEFManager.InitializeCef();

            new ControlManager();

            LogManager.WriteLog("Core Initialized");

            var browser = new Browser(new Microsoft.ClearScript.V8.V8ScriptEngine(), "https://www.youtube.com/watch?v=ufQl2NCzf6E", Screen, false);
            CefController.ShowCursor = true;

            base.OnInit();
        }

        internal void StartMainMenu()
        {
            LogManager.WriteLog("Enter on Start Main Menu");
            InStartMenu = true;

            Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);
            Game.FadeScreenIn(1000);
            Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, 1, 0, 0);
            Function.Call(Hash._SHOW_LOADING_SCREEN, 1122662550, 347053089, 0, "RED DEAD REDEMPTION II", "Network", "Loading...");

            World.CurrentDayTime = new TimeSpan(12, 0, 0);

            Model model = new Model(PedHash.A_C_Horse_Turkoman_DarkBay);
            Console.WriteLine("model request");
            model.Request();
            
            Console.WriteLine("Model changed: " + Game.Player.ChangeModel(model).ToString());
        }

        private static bool firstTick;
        public override void OnTick()
        {
            if (!firstTick)
            {
                firstTick = true;
                StartMainMenu();
            }

            if (InStartMenu)
            {
                World.CurrentWeather = WeatherType.Sunny;
                Function.Call(Hash.PAUSE_CLOCK, true);


                Function.Call(Hash._SET_MOUSE_CURSOR_ACTIVE_THIS_FRAME);

                //Game.DisableAllControlsThisFrame(2);
                /*
                Game.EnableControlThisFrame(0, Control.CursorX);
                Game.EnableControlThisFrame(0, Control.CursorY);
                */
                var res = Main.Screen;

                var mouseX = Game.GetControlNormal(0, Control.CursorX) * res.Width;
                var mouseY = Game.GetControlNormal(0, Control.CursorY) * res.Height;
            }

            Game.TimeScale = 1;

            Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);

            World.CurrentDayTime = new TimeSpan(12, 0, 0);

            base.OnTick();
        }
    }
}
