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
using RDRN_Core.Utils;
using RDRN_Module.Math;
using System.Reflection;
using System.Collections.Generic;

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

            LogManager.WriteLog("Control Manager Initializing");
            new ControlManager();
            


            LogManager.WriteLog("Core Initialized");
        }

        internal void StartMainMenu()
        {
            LogManager.WriteLog("Enter on Start Main Menu");
            InStartMenu = true;
            Game.Player.Character.Position = new Vector3(0, 0, 70);
            Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);
            Game.FadeScreenIn(1000);
            Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, 1, 0, 0);

            World.CurrentWeather = WeatherType.Sunny;
            Function.Call(Hash.PAUSE_CLOCK, true);

            World.CurrentDayTime = new TimeSpan(12, 0, 0);
            
            Model model = new Model(PedHash.A_C_Horse_Turkoman_DarkBay);
            Console.WriteLine("model request");
            model.Request();
            Console.WriteLine("Model changed: " + Game.Player.ChangeModel(model).ToString());
            var browser = new Browser(new Microsoft.ClearScript.V8.V8ScriptEngine(), "https://www.youtube.com/watch?v=ufQl2NCzf6E", Screen, false);
            CefController.ShowCursor = true;


            Game.TimeScale = 1;

            //Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);


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



            }



        }
    }
}
