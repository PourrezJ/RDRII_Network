using RDRN_Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.Marshal;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RDRN_Core.Native;
using EasyHook;
using System.Runtime.InteropServices;

namespace RDRN_Core
{
    internal class NativeHook
    {
        /*
        private HookWrapper<WaitHookDelegate> WaitHook;
        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate void WaitHookDelegate(int wait);

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate ulong GetHandleAddressFuncDelegate(Native.Hash hash);

        static GetHandleAddressFuncDelegate GetNativAddressFunc;
        */
        internal NativeHook()
        {
            unsafe
            {
                /*anything useful (at least nothing visible);
                 * so a hook omitting this switch will be handled one or two orders of magnitudes faster until finally your handler gains execution.
                 * But as a managed hook is still executed within at last 1000 nano-seconds, even the "slow" managed implementation will be fast enough in most cases.
                 * With C++.NET you would be able to provide such native high-speed hooks for frequently called API methods, while still using managed ones for usual API methods, within a single assembly!
                 * A pure unmanaged, empty hook executes in approx. 70 nano-seconds, which is incredible fast considering the thread deadlock barrier and thread ACL negotiation that are already included in this benchmark!
                 * */
                //var test = Function.GetCommandHandler(Native.Hash.WAIT);

                //Console.WriteLine(new IntPtr(test));
                //var get_native_address = Memory.FindPattern("\x0F\xB6\xC1\x48\x8D\x15\x00\x00\x00\x00\x4C\x8B\xC9", "xxxxxx????xxx");

                ////ProcessModule module = Process.GetCurrentProcess().MainModule;

                ////ulong address = (ulong)module.BaseAddress.ToInt64();

                //GetNativAddressFunc = GetDelegateForFunctionPointer<GetHandleAddressFuncDelegate>(new IntPtr(get_native_address));

                //Console.WriteLine("GetNativAddressFunc 1: " + (ulong)GetNativAddressFunc(Native.Hash.WAIT));

                //var localHook = LocalHook.CreateUnmanaged(GetNativAddressFunc(Native.Hash.WAIT), proxy, callback);

                //WaitHook = new HookWrapper<WaitHookDelegate>((IntPtr)test, new WaitHookDelegate(WaitNative), this);

                ////WaitNative(999999);
            }
        }

        private void WaitNative(int wait)
        {
            /*
            Console.WriteLine("Wait");
            WaitHook.Target(wait);*/
        }
    }
}
