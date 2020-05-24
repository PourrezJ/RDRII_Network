using RDRN_Core.Gui.Cef;
using RDRN_Core.Gui.DirectXHook;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

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

    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public POINT pt;
        public int hwnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    internal class ControlManager
    {
        internal static PointF MousePos { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);


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

                    if (ScreenToClient(DxHook.SwapChain.Description.OutputHandle, ref point))
                    {
                        MousePos = new PointF((point.X < 0 ? 0 : point.X) , (point.Y < 0 ? 0 : point.Y));

                        if (CEFManager.Cursor != null)
                        {
                            CEFManager.Cursor.Position = MousePos;
                        }

                        //Console.WriteLine((MousePos.X).ToString() + " " + (MousePos.Y).ToString());
                    } 
                }
                Thread.Sleep(10);
            }
        }
    }
}
