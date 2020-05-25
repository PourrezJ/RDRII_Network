using Microsoft.ClearScript.V8;
using RDRN_Module;
using System;
using System.Drawing;
using Xilium.CefGlue;
using Xilium.CefGlue.Platform.Windows;

namespace RDRN_Core.Gui.Cef
{
    internal class Browser : IDisposable
    {
        internal MainCefClient Client;
        internal CefBrowser browser;
        internal BrowserJavascriptCallback Callback;
        internal CefV8Context MainContext;
        internal bool HasFocused;

        internal readonly bool _localMode;
        

        public CefBrowserHost GetHost()
            => browser.GetHost();

        private bool _headless = false;
        public bool Headless
        {
            get => _headless;
            set
            {
                Client.SetHidden(value);
                _headless = value;
            }
        }
        private Point _position;

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                Client.SetPosition(value.X, value.Y);
            }
        }

        public PointF[] Pinned { get; set; }

        private Size _size;
        public Size Size
        {
            get => _size;
            set
            {
                Client.SetSize(value.Width, value.Height);
                _size = value;
            }
        }

        private V8ScriptEngine Father;

        public void Eval(string code)
        {
            if (!_localMode) return;
            browser.GetMainFrame().ExecuteJavaScript(code, null, 0);
        }

        public void Call(string method, params object[] arguments)
        {
            if (!_localMode) 
                return;

            string callString = method + "(";
            if (arguments != null)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    string comma = ", ";
                    if (i == arguments.Length - 1)
                        comma = "";
                    if (arguments[i] is string)
                    {
                        var escaped = System.Web.HttpUtility.JavaScriptStringEncode(arguments[i].ToString(), true);
                        callString += escaped + comma;
                    }
                    else if (arguments[i] is bool)
                    {
                        callString += arguments[i].ToString().ToLower() + comma;
                    }
                    else
                    {
                        callString += arguments[i] + comma;
                    }
                }
            }
            callString += ");";

            browser.GetMainFrame().ExecuteJavaScript(callString, null, 0);
        }

        internal Browser(V8ScriptEngine father, string url, Size browserSize, bool localMode)
        {
            try
            {
                Father = father;

                LogManager.WriteLog("--> Browser: Start");
                var windowInfo = CefWindowInfo.Create();
                windowInfo.SetAsWindowless(IntPtr.Zero, true);
                windowInfo.SetAsPopup(IntPtr.Zero, null);
                windowInfo.Width = browserSize.Width;
                windowInfo.Height = browserSize.Height;
                windowInfo.WindowlessRenderingEnabled = true;
                //windowInfo.SharedTextureEnabled = true;
               // windowInfo.ExternalBeginFrameEnabled = true; // if true the acceleratedPaint is not called

                windowInfo.Style = WindowStyle.WS_OVERLAPPEDWINDOW | WindowStyle.WS_CLIPCHILDREN;

                var browserSettings = new CefBrowserSettings()
                {
                    BackgroundColor = new CefColor(0, 0, 0, 0),
                    JavaScript = CefState.Enabled,
                    JavaScriptAccessClipboard = CefState.Disabled,
                    JavaScriptCloseWindows = CefState.Disabled,
                    JavaScriptDomPaste = CefState.Disabled,
                    //JavaScriptOpenWindows = CefState.Disabled,
                    LocalStorage = CefState.Disabled,
                    WindowlessFrameRate = 60

                };

                Client = new MainCefClient(this, browserSize.Width, browserSize.Height);

                Size = browserSize;
                _localMode = localMode;
                Callback = new BrowserJavascriptCallback(father, this);

                Client.OnCreated += (sender, args) =>
                {
                    browser = sender as CefBrowser;
                    LogManager.WriteLog("-> Browser created!");
                    GoToPage("https://www.twitch.tv/directory");
                    LogManager.WriteLog("-> Browser created! 2");
                };
                LogManager.WriteLog("--> Browser: Creating Browser");
                CefBrowserHost.CreateBrowser(windowInfo, Client, browserSettings, url);

                lock (CEFManager.Browsers)
                {
                    CEFManager.Browsers.Add(this);
                }
                
            }
            catch (Exception e)
            {
                LogManager.Exception(e, "CreateBrowser Error");
            }
           
            LogManager.WriteLog("--> Browser: End");
        }

        internal void GoToPage(string page)
        {
            if (browser != null)
            {
                LogManager.WriteLog("Trying to load page " + page + "...");
                browser.GetMainFrame().LoadUrl(page);
                LogManager.WriteLog("Page loaded ...");
            }
        }

        internal void GoBack()
        {
            if (browser != null && browser.CanGoBack)
            {
                LogManager.WriteLog("Trying to go back a page...");
                browser.GoBack();
            }
        }

        internal void Close()
        {
            Client.Close();

            if (browser == null) return;
            var host = browser.GetHost();
            host.CloseBrowser(true);
            host.Dispose();
            browser.Dispose();
        }

        internal string GetAddress()
        {
            if (browser == null) 
                return null;
            return browser.GetMainFrame().Url;
        }

        internal bool IsLoading() => browser.IsLoading;

        internal bool IsInitialized() => browser != null;

        public void Dispose() => browser = null;
    }
}
