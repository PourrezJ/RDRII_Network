using RDRN_Module;
using RDRN_Module.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
    public class Main : Script
    {
        public override void OnInit()
        {
            /*
            unsafe
            {
                byte* address = Memory.FindPattern("\x48\x8B\x05\x00\x00\x00\x00\x41\x0F\xBF\xC8\x0F\xBF\x40\x10", "xxx????xxxxxxxx");
                var PedPoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);

                if (PedPoolAddress != null)
                {

                    Console.WriteLine("Penis");
                }
            }
            */
            base.OnInit();
        }

        bool test = false;
        public override void OnTick()
        {
            if (!test)
            {
                test = true;
                Console.WriteLine("On Tick");
            }
            Function.Call(Hash.DRAW_RECT, 0.1f, 0.2f, 0.1f, 0.1f, 255, 0, 0, 255);

            base.OnTick();
        }
    }
}
