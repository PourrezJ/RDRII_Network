using RDRN_Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.Marshal;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RDRN_Core
{
    internal class NativeHook
    {
        private HookWrapper<WaitHookDelegate> WaitHook;

        delegate void WaitHookDelegate(int wait);

        delegate ulong GetHandleAddressFuncDelegate(Native.Hash hash);
        static GetHandleAddressFuncDelegate GetNativAddressFunc;

        internal NativeHook()
        {
            unsafe
            {
                //var get_native_address = Memory.FindPattern("\x0F\xB6\xC1\x48\x8D\x15\x00\x00\x00\x00\x4C\x8B\xC9", "xxxxxx????xxx");

                //ProcessModule module = Process.GetCurrentProcess().MainModule;

                //ulong address = (ulong)module.BaseAddress.ToInt64();

                //GetNativAddressFunc = GetDelegateForFunctionPointer<GetHandleAddressFuncDelegate>(new IntPtr(get_native_address));

                //Console.WriteLine("GetNativAddressFunc 1: " + (ulong)GetNativAddressFunc(Native.Hash.WAIT));

                //WaitHook = new HookWrapper<WaitHookDelegate>((IntPtr)GetNativAddressFunc(Native.Hash.PLAYER_PED_ID), new WaitHookDelegate(WaitNative), this);

                ////WaitNative(999999);
            }
        }

        private void WaitNative(int wait)
        {
            Console.WriteLine("Wait");
            WaitHook.Target(wait);
        }
    }
}
