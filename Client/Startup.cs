using Memory;
using RDRN_Core.Gui.Cef;
using RDRN_Core.Gui.DirectXHook;
using RDRN_Core.Native;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms = System.Windows.Forms;

namespace RDRN_Core
{
    public static class Startup
    {
        static ScriptDomain Domain;

        public static string RDRN_Path { get; private set; }

        public static DxHook DxHook { get; private set; }

        public static Mem MemLib = new Mem();

        public static void OnPreInit(string path)
        {
            var path2 = new DirectoryInfo(path);
            RDRN_Path = path2.Parent.FullName;

            LogManager.WriteLog(LogLevel.Information, "Core Initializing");

            LogManager.WriteLog(LogLevel.Trace, "RDRNetwork Path: " + RDRN_Path);

            LogManager.WriteLog(LogLevel.Information, "PrepareNetwork configuration");
            //PrepareNetwork();

            MemLib.OpenProcess("RDR2");

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                LogManager.WriteLog("Initializing DxHook.");
                DxHook = new DxHook();
                LogManager.WriteLog("Initializing CEF.");
                CEFManager.InitializeCef();
                LogManager.WriteLog("Hook adress.");
                long myAoBScan = (await MemLib.AoBScan("eb ? 90 ef e8 ? ? ? ? 48 83 c4", false, false)).FirstOrDefault();
                LogManager.DebugLog("Our First Found Address is " + myAoBScan);
                //Hook();
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

        public static void Hook()
        {
            unsafe
            {
                var pat = NativeMemory.FindPattern("\xeb\x00\x90\xef\xe8\x00\x00\x00\x00\x48\x83\xc4", "x?xxx????xxx");
                IntPtr adress = new IntPtr(pat - 0x1C);
                NativeMemory.IsBitSet(adress, 1);
                //Console.Debug(new IntPtr(pat).ToString());
            }
        }
    }
}