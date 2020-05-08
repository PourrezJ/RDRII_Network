using RDRN_Core.Gui.DirectXHook;
using RDRN_Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RDRN_Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    internal class ControlManager
    {
        internal static PointF MousePos { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        internal ControlManager()
        {
            Thread thread = new Thread(new ThreadStart(ControlThread));
            thread.Start();
        }

        private void ControlThread()
        {
            while (true)
            {
                if (DxHook.Started)
                {
                    GetCursorPos(out POINT point);
                    GetWindowRect(DxHook.SwapChain.Description.OutputHandle, out RECT rect);

                    var x = point.X - rect.Left;
                    var y = point.Y - rect.Top;

                    if (x > rect.Right)
                        x = rect.Right;

                    if (y > rect.Bottom)
                        y = rect.Bottom;

                    MousePos = new PointF(x < 0 ? 0 : x, y < 0 ? 0 : y);

                    //Console.WriteLine((MousePos.X).ToString() + " " + (MousePos.Y).ToString());
                }
            }
        }
    }
}
