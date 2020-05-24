﻿using RDRN_API;
using RDRN_Core.Utils;
using RDRN_Module;
using RDRN_Module.Native;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xilium.CefGlue;
using Control = RDRN_API.Control;

namespace RDRN_Core.Gui.Cef
{
    internal class CefController : Script
    {
        private static bool _showCursor;

        public static bool ShowCursor
        {
            get => _showCursor;
            set
            {
                if (!_showCursor && value)
                {
                    _justShownCursor = true;
                    _lastShownCursor = Util.TickCount;
                }
                _showCursor = value;

                CEFManager.SetMouseHidden(!value);
            }
        }

        private static bool _justShownCursor;
        private static long _lastShownCursor = 0;
        private static Keys _lastKey;

        internal static CefEventFlags GetMouseModifiers(bool leftbutton, bool rightButton)
        {
            CefEventFlags mod = CefEventFlags.None;

            if (leftbutton) mod |= CefEventFlags.LeftMouseButton;
            if (rightButton) mod |= CefEventFlags.RightMouseButton;

            return mod;
        }

        public override void OnTick()
        {
            /*
            lock (CEFManager.Browsers) // needed for accelerated Paint
            {
                for(int a = 0; a < CEFManager.Browsers.Count; a++)
                {
                    //CEFManager.Browsers[a].GetHost().SendExternalBeginFrame();
                }
            }
            */
            if (ShowCursor)
            {
                Game.DisableAllControlsThisFrame(0);

                var screen = Main.Screen;

                var mousePos = CEFManager.Cursor?.Position ?? new PointF();
                var mouseX = mousePos.X/* * screen.Width*/;
                var mouseY = mousePos.Y /** screen.Height*/;


                var mouseDown = Game.IsDisabledControlJustPressed(0, Control.Attack);
                var mouseDownRN = Game.IsDisabledControlJustPressed(0, Control.Attack);
                var mouseUp = Game.IsDisabledControlJustPressed(0, Control.Attack);

                var rmouseDown = Game.IsDisabledControlJustPressed(0, Control.Aim);
                var rmouseDownRN = Game.IsDisabledControlJustPressed(0, Control.Aim);
                var rmouseUp = Game.IsDisabledControlJustPressed(0, Control.Aim);

                var wumouseDown = Game.IsDisabledControlJustPressed(0, Control.CursorScrollUp);
                var wdmouseDown = Game.IsDisabledControlJustPressed(0, Control.CursorScrollDown);

                foreach (var browser in CEFManager.Browsers)
                {
                    if (!browser.IsInitialized()) 
                        continue;

                    if (!browser._hasFocused)
                     {
                         browser._browser.GetHost().SetFocus(true);
                         browser._browser.GetHost().SendFocusEvent(true);
                         browser._hasFocused = true;
                     }

                     if (mouseX > browser.Position.X && mouseY > browser.Position.Y &&
                         mouseX < browser.Position.X + browser.Size.Width &&
                         mouseY < browser.Position.Y + browser.Size.Height)
                     {
                         var ev = new CefMouseEvent((int)(mouseX - browser.Position.X), (int)(mouseY - browser.Position.Y),
                                 GetMouseModifiers(mouseDownRN, rmouseDownRN));

                         browser._browser
                             .GetHost()
                             .SendMouseMoveEvent(ev, false);

                        if (mouseDown)
                        {
                            browser._browser
                            .GetHost()
                            .SendMouseClickEvent(ev, CefMouseButtonType.Left, false, 1);
                        }

                         if (mouseUp)
                             browser._browser
                                 .GetHost()
                                 .SendMouseClickEvent(ev, CefMouseButtonType.Left, true, 1);

                         if (rmouseDown)
                             browser._browser
                                 .GetHost()
                                 .SendMouseClickEvent(ev, CefMouseButtonType.Right, false, 1);

                         if (rmouseUp)
                             browser._browser
                                 .GetHost()
                                 .SendMouseClickEvent(ev, CefMouseButtonType.Right, true, 1);

                         if (wdmouseDown)
                             browser._browser
                                 .GetHost()
                                 .SendMouseWheelEvent(ev, 0, -30);

                         if (wumouseDown)
                             browser._browser
                                 .GetHost()
                                 .SendMouseWheelEvent(ev, 0, 30);
                     }
                 }
            }
            /*
            else
            {
                Function.Call(Hash._SET_MOUSE_CURSOR_ACTIVE_THIS_FRAME);
            }*/
        }



        public override void OnKeyDown(KeyEventArgs args)
        {
            
            if (!ShowCursor) return;

            if (_justShownCursor && Util.TickCount - _lastShownCursor < 500)
            {
                _justShownCursor = false;
                return;
            }

            foreach (var browser in CEFManager.Browsers)
            {
                if (!browser.IsInitialized())
                    continue;

                CefEventFlags mod = CefEventFlags.None;
                
                if (args.Control) mod |= CefEventFlags.ControlDown;
                if (args.Shift) mod |= CefEventFlags.ShiftDown;
                if (args.Alt) mod |= CefEventFlags.AltDown;
                
                CefKeyEvent kEvent = new CefKeyEvent();
                kEvent.EventType = CefKeyEventType.KeyDown;
                kEvent.Modifiers = mod;
                kEvent.WindowsKeyCode = (int)args.KeyCode;
                kEvent.NativeKeyCode = (int)args.KeyValue;
                browser._browser.GetHost().SendKeyEvent(kEvent);

                CefKeyEvent charEvent = new CefKeyEvent();
                charEvent.EventType = CefKeyEventType.Char;
                
                var key = args.KeyCode;

                if ((key == Keys.ShiftKey && _lastKey == Keys.Menu) ||
                    (key == Keys.Menu && _lastKey == Keys.ShiftKey))
                {
                    //ClassicChat.ActivateKeyboardLayout(1, 0);
                    return;
                }

                _lastKey = key;

                if (key == Keys.Escape)
                {
                    return;
                }
                /*
                var keyChar = ClassicChat.GetCharFromKey(key, Game.IsKeyPressed(Keys.ShiftKey), Game.IsKeyPressed(Keys.Menu) && Game.IsKeyPressed(Keys.ControlKey));

                if (keyChar.Length == 0 || keyChar[0] == 27) return;

                charEvent.WindowsKeyCode = keyChar[0];*/
                charEvent.Modifiers = mod;
                browser._browser.GetHost().SendKeyEvent(charEvent);
            }
        }

        public override void OnKeyUp(KeyEventArgs args)
        {
            if (!ShowCursor) 
                return;

            foreach (var browser in CEFManager.Browsers)
            {
                if (!browser.IsInitialized()) 
                    continue;

                CefKeyEvent kEvent = new CefKeyEvent();
                kEvent.EventType = CefKeyEventType.KeyUp;
                kEvent.WindowsKeyCode = (int)args.KeyCode;
                browser._browser.GetHost().SendKeyEvent(kEvent);
            }
        }

        public override void OnMouseDown(MouseButtons button)
        {
            Console.WriteLine(button.ToString());
        }
    }
}