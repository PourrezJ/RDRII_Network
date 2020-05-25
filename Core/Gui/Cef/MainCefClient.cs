using RDRN_Module;
using System;
using System.Drawing;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class MainCefClient : CefClient
    {
        internal readonly MainCefLoadHandler _loadHandler;
        internal readonly MainCefRenderHandler _renderHandler;
        internal readonly MainLifeSpanHandler _lifeSpanHandler;
        internal readonly ContextMenuRemover _contextMenuHandler;

        internal event EventHandler OnCreated;

        internal PointF Position;

        public MainCefClient(Browser browser, int windowWidth, int windowHeight)
        {
            _renderHandler = new MainCefRenderHandler(browser, windowWidth, windowHeight);
            _loadHandler = new MainCefLoadHandler();
            _lifeSpanHandler = new MainLifeSpanHandler(this);
            _contextMenuHandler = new ContextMenuRemover();
            LogManager.WriteLog("-> MainCefClient");
        }

        public void SetPosition(int x, int y)
        {
            Position = new PointF(x, y);
            _renderHandler.SetPosition(x, y);
        }

        public void SetSize(int w, int h)
        {
            _renderHandler.SetSize(w, h);
        }

        public void SetHidden(bool hidden)
        {
           _renderHandler.SetHidden(hidden);
        }

        public void Close()
        {
            _renderHandler.Dispose();
        }

        public void Created(CefBrowser bs)
        {
            OnCreated?.Invoke(bs, EventArgs.Empty);
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            LogManager.WriteLog("-> _contextMenuHandler");
            return _contextMenuHandler;
        }
        
        protected override CefRenderHandler GetRenderHandler()
        {
            //LogManager.CefLog("-> _renderHandler");
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            LogManager.WriteLog("-> _loadHandler");
            return _loadHandler;
        }
        
        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            LogManager.WriteLog("-> _lifeSpanHandler");
            return _lifeSpanHandler;
        }
    }
}
