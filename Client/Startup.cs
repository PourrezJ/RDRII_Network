using RDRN_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
    public static class Startup
    {
        static ScriptDomain Domain;

        public static string RDRN_Path { get; private set; }

        public static void OnPreInit(string path)
        {
            var path2 = new DirectoryInfo(path);
            RDRN_Path = path2.FullName;

            LogManager.WriteLog(LogLevel.Information, "Core Initializing");

            LogManager.WriteLog(LogLevel.Trace, "RDRNetwork Path: " + path);

            LogManager.WriteLog(LogLevel.Information, "PrepareNetwork configuration");
            //PrepareNetwork();

            
        }

        public static bool Init()
        {
            Domain = ScriptDomain.Load(RDRN_Path);
            
            if (Domain != null)
                Domain.Start();

            LogManager.WriteLog(LogLevel.Information, "Core Initialized");
            return true;
        }

        public static void Tick()
        {
            Domain?.DoTick();
        }
    }
}