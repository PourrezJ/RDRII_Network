using RDRN_Core.Gui.Cef;
using System.IO;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

namespace RDRN_Core
{
    public static class Startup
    {
        static ScriptDomain Domain;

        public static string RDRN_Path { get; private set; }

        public static void OnPreInit(string path)
        {
            var path2 = new DirectoryInfo(path);
            RDRN_Path = path2.Parent.FullName;

            LogManager.WriteLog(LogLevel.Information, "Core Initializing");

            LogManager.WriteLog(LogLevel.Trace, "RDRNetwork Path: " + RDRN_Path);

            LogManager.WriteLog(LogLevel.Information, "PrepareNetwork configuration");
            //PrepareNetwork();

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                LogManager.WriteLog("Initializing CEF.");
                CEFManager.InitializeCef();
            });
        }

        public static bool Init()
        {
            Domain = ScriptDomain.Load(Path.Combine(RDRN_Path, "bin\\Scripts"));
            
            if (Domain != null)
                Domain.Start();

            LogManager.WriteLog(LogLevel.Information, "Core Initialized");
            /*
            Task.Run(() =>
            {
                while (true)
                {
                    Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);
                }
            });*/
            
            return true;
        }

        public static void Tick()
        {
            Domain?.DoTick();
        }

        public static void KeyboardMessage(WinForms.Keys key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
        {
            Domain?.DoKeyboardMessage(key, status, statusCtrl, statusShift, statusAlt);
        }
    }
}