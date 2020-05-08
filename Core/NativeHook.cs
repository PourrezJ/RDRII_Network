using RDRN_Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
    internal class NativeHook
    {
        private HookWrapper<WaitHookDelegate> WaitHook;

        delegate void WaitHookDelegate(int wait);

        internal void Init()
        {
            //WaitHook = new HookWrapper<WaitHookDelegate>(address[54], new WaitHookDelegate(WaitNative), this);
        }

        private void WaitNative(int wait)
        {
            
        }
    }
}
