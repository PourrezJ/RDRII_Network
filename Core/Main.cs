using RDRN_Core;
using RDRN_Core.Native;
using RDRN_Core.Gui.Cef;
using RDRN_Module;
using RDRN_Core.Gui.DirectXHook;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace RDRN_Core
{
    public partial class Main
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

        public static void OnPreInit(string path)
        {
            RDRNetworkPath = System.IO.Directory.GetParent(path).FullName;
            LogManager.WriteLog(LogLevel.Information, "Core Initializing");

            LogManager.WriteLog(LogLevel.Trace, "RDRNetwork Path: " + RDRNetworkPath);
         
            LogManager.WriteLog(LogLevel.Information, "PrepareNetwork configuration");
            PrepareNetwork();

            LogManager.WriteLog(LogLevel.Information, "Core Initialized");
        }

        public static bool OnInit()
        {
            LogManager.WriteLog(LogLevel.Information, "DirectX hook Initializing");
            DxHook = new DxHook();

            LogManager.WriteLog(LogLevel.Information, "Cef Initializing");
            CEFManager.InitializeCef();

            LogManager.WriteLog(LogLevel.Information, "Control Manager Initializing");
            new ControlManager();

            Task.Run(() =>
            {
                while(true)
                {
                    Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);
                }
            });

            return true;
        }

        internal static void StartMainMenu()
        {
            try
            {
                new NativeHook();

                LogManager.WriteLog(LogLevel.Information, "Enter on Start Main Menu");
                InStartMenu = true;
               // Game.Player.Character.Position = new Vector3(0, 0, 70);
                
                Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);
                Game.FadeScreenIn(1000);
                Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, 1, 0, 0);

                World.CurrentWeather = WeatherType.Sunny;
                Function.Call(Hash.PAUSE_CLOCK, true);

                World.CurrentDayTime = new TimeSpan(12, 0, 0);
                
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

                LogManager.WriteLog(LogLevel.Trace, "Native 100k result: " + result);

            }
            catch(Exception ex)
            {
                LogManager.Exception(ex);
            }
        }

        public static void OnTick()
        {
            if (!firstTick)
            {

                firstTick = true;

                StartMainMenu();
            }
            //Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);
            if (InStartMenu)
            {



            }
        }
    }
}
