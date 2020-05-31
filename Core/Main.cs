using RDRN_API;
using RDRN_API.Native;
using RDRN_Core.Gui.Cef;
using RDRN_Module;
using RDRN_Module.Native;
using RDRN_Core.Gui.DirectXHook;
using System;
using System.Drawing;
using RDRN_Module.Math;
using System.Reflection;

namespace RDRN_Core
{
    public partial class Main : Script
    {
        internal static bool InStartMenu;

        internal static string RDRNetworkPath;
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

        internal void StartMainMenu()
        {
            try
            {
                LogManager.WriteLog("Enter on Start Main Menu");
                InStartMenu = true;
               // Game.Player.Character.Position = new Vector3(0, 0, 70);
                
                Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);
                Game.FadeScreenIn(1000);
                Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, 1, 0, 0);

                World.CurrentWeather = WeatherType.Sunny;
                Function.Call(Hash.PAUSE_CLOCK, true);

                World.CurrentDayTime = new TimeSpan(12, 0, 0);
                
                Console.WriteLine("model request");
                Model model = new Model(PedHash.CS_abigailroberts);

                model.Request(250);

                Console.WriteLine("Model is Loaded:" + model.IsLoaded);

                Game.Player.ChangeModel(model);

                //Console.WriteLine("Model changed: " + changed.ToString());
                //var browser = new Browser(new Microsoft.ClearScript.V8.V8ScriptEngine(), "https://www.youtube.com/watch?v=ufQl2NCzf6E", Screen, false);
                //CefController.ShowCursor = true;

                Game.TimeScale = 1;

                var time = DateTime.Now;

                for(int a = 0; a < 100000; a++)
                {
                    Function.Call<int>(Hash.PLAYER_ID);
                    Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);
                }

                var result = (DateTime.Now - time).TotalMilliseconds;

                Console.WriteLine("Native 100k result: " + result);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override void OnTick()
        {
            if (!firstTick)
            {

                firstTick = true;

                StartMainMenu();
            }
            Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);
            if (InStartMenu)
            {



            }
        }

        public override void OnInit()
        {
            RDRNetworkPath = System.IO.Directory.GetParent(CurrentDir).FullName;

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
            //base.OnInit();

            
        }
    }
}
