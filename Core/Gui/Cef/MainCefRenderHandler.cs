using System;
using System.Security;
using Xilium.CefGlue;
using RDRN_Module;
using RDRN_Core.Gui.DirectXHook;
using SharpDX.DXGI;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;

namespace RDRN_Core.Gui.Cef
{
    internal class MainCefRenderHandler : CefRenderHandler
    {
        private int windowHeight;

        internal static ImageElement ImageElement;

        private int windowWidth;

        private Browser browser;

        internal static List<MainCefRenderHandler> CefRenderHandlers = new List<MainCefRenderHandler>();

        public MainCefRenderHandler(Browser browser, int windowWidth, int windowHeight)
        {
            this.browser = browser;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            ImageElement = new ImageElement();

            CefRenderHandlers.Add(this);

            LogManager.WriteLog(LogLevel.Trace, "-> Instantiated Renderer");
        }

        public void SetHidden(bool hidden)
        {
           // imageElement.Hidden = hidden;
        }

        public void SetSize(int width, int height)
        {
            windowHeight = height;
            windowWidth = width;
        }

        public void SetPosition(int x, int y)
        {
           // imageElement.Position = new Point(x, y);
        }

        public void Dispose()
        {
            CefRenderHandlers.Remove(this);
            ImageElement?.Dispose();
            ImageElement = null;
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {

        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return true;
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return true;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }
        /*
        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (this.browser == null)
                return;


            if (imageElement != null)
            {
                lock (imageElement)
                {
                    imageElement.SetBitmap(new System.Drawing.Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, buffer), this.browser.Position);
                }
            }
        }*/
        
        [SecurityCritical]
        protected override unsafe void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (this.browser == null)
                return;
            
            if (ImageElement == null)
                return;

            int stride = width * sizeof(int);
            var length = height * stride;

            using (var tempStream = new SharpDX.DataStream(height * stride, true, true))
            {
                for (int y = 0; y < height; y++)
                {
                    int offset = stride * y;
                    for (int x = 0; x < width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(buffer, offset++);
                        byte G = Marshal.ReadByte(buffer, offset++);
                        byte R = Marshal.ReadByte(buffer, offset++);
                        byte A = Marshal.ReadByte(buffer, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        tempStream.Write(rgba);
                    }
                }

                tempStream.Position = 0;
                ImageElement.Position = this.browser.Position;
                ImageElement.Height = height;
                ImageElement.Width = width;

                if (DxHook.CurrentRenderTarget2D1 != null)
                {
                    ImageElement.D2D1Bitmap = new SharpDX.Direct2D1.Bitmap(
                            DxHook.CurrentRenderTarget2D1,
                            new SharpDX.Size2(width, height),
                            tempStream,
                            stride,
                            new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

                }

                tempStream.Close();
                tempStream.Dispose();
            }
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {

        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {

        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            return null;
        }

        protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
        {
            //Console.WriteLine("GetViewRect");
            rect = new CefRectangle(0, 0, windowWidth, windowHeight);
        }

        protected unsafe override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
            return;
            if (DxHook.CurrentRenderTarget2D1 == null)
                return;
            /*
            try
            {
                
                ResourceShared = DxHook.Device11.OpenSharedResource<SharpDX.Direct3D11.Resource>(sharedHandle);
                
                var texture = ResourceShared.QueryInterface<SharpDX.Direct3D11.Texture2D>();

                using (var copy = GetCopy(texture))
                using (var surface = copy.QueryInterface<SharpDX.DXGI.Surface>())
                {
                    // can't destroy the surface now with WARP driver
                    DataStream ds;
                    var db = surface.Map(SharpDX.DXGI.MapFlags.Read, out ds);

                    int w = texture.Description.Width;
                    int h = texture.Description.Height;
                    var wb = new System.Windows.Media.Imaging.WriteableBitmap(w, h, 96.0, 96.0, PixelFormats.Bgra32, null);
                    wb.Lock();
                    try
                    {
                        uint* wbb = (uint*)wb.BackBuffer;

                        ds.Position = 0;
                        for (int y = 0; y < h; y++)
                        {
                            ds.Position = y * db.Pitch;
                            for (int x = 0; x < w; x++)
                            {
                                var c = ds.Read<uint>();
                                wbb[y * w + x] = c;
                            }
                        }
                        ds.Dispose();
                    }
                    finally
                    {
                        wb.AddDirtyRect(new Int32Rect(0, 0, w, h));
                        wb.Unlock();
                    }
                }
                
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex);
            }*/


            //Console.WriteLine("OnAcceleratedPaint");
        }

        private SharpDX.Direct3D11.Texture2D OpenSharedResource(IntPtr handle)
        {
            var resource = DxHook.Device11.OpenSharedResource<SharpDX.Direct3D11.Resource>(handle);
            var texture = resource.QueryInterface<SharpDX.Direct3D11.Texture2D>();
            return texture;
        }

        public unsafe WriteableBitmap GetBitmap(SharpDX.Direct3D11.Texture2D tex)
        {
            DataRectangle db;
            using (var copy = GetCopy(tex))
            using (var surface = copy.QueryInterface<SharpDX.DXGI.Surface>())
            {
                // can't destroy the surface now with WARP driver
                DataStream ds;
                db = surface.Map(SharpDX.DXGI.MapFlags.Read, out ds);

                int w = tex.Description.Width;
                int h = tex.Description.Height;
                var wb = new System.Windows.Media.Imaging.WriteableBitmap(w, h, 96.0, 96.0, PixelFormats.Bgra32, null);
                wb.Lock();
                try
                {
                    uint* wbb = (uint*)wb.BackBuffer;

                    ds.Position = 0;
                    for (int y = 0; y < h; y++)
                    {
                        ds.Position = y * db.Pitch;
                        for (int x = 0; x < w; x++)
                        {
                            var c = ds.Read<uint>();
                            wbb[y * w + x] = c;
                        }
                    }
                    ds.Dispose();
                }
                finally
                {
                    wb.AddDirtyRect(new Int32Rect(0, 0, w, h));
                    wb.Unlock();
                }
                return wb;
            }
        }

        private static SharpDX.Direct3D11.Texture2D GetCopy(SharpDX.Direct3D11.Texture2D tex)
        {
            var teximg = new SharpDX.Direct3D11.Texture2D(tex.Device, new SharpDX.Direct3D11.Texture2DDescription
            {
                Usage = SharpDX.Direct3D11.ResourceUsage.Staging,
                BindFlags = SharpDX.Direct3D11.BindFlags.None,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                ArraySize = tex.Description.ArraySize,
                Height = tex.Description.Height,
                Width = tex.Description.Width,
                MipLevels = tex.Description.MipLevels,
                SampleDescription = tex.Description.SampleDescription,
            });
            tex.Device.ImmediateContext.CopyResource(tex, teximg);
            return teximg;
        }
    }
}