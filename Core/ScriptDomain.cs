using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
    internal class ScriptDomain
    {
        internal static ScriptDomain Instance;

        internal AppDomain CurrentAppDomain;

        ScriptDomain()
        {
            Instance = this;

            CurrentAppDomain = AppDomain.CurrentDomain;


        }


    }
}
