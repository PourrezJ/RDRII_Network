using RDRN_Module;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class WebKitInjector : CefRenderProcessHandler
    {
        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            LogManager.WriteLog("-> OnContextCreated!");
            if (frame.IsMain)
            {
                LogManager.WriteLog("-> Setting main context!");

                Browser father = CefUtil.GetBrowserFromCef(browser);
                if (father != null)
                {
                    father._mainContext = context;
                    LogManager.WriteLog("-> Main context set!");
                }
            }

            CefV8Value global = context.GetGlobal();

            CefV8Value func = CefV8Value.CreateFunction("resourceCall", new V8Bridge(browser));
            global.SetValue("resourceCall", func);

            CefV8Value func2 = CefV8Value.CreateFunction("resourceEval", new V8Bridge(browser));
            global.SetValue("resourceEval", func2);

            base.OnContextCreated(browser, frame, context);
        }
        /*
        protected override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        {
            if ((request.TransitionType & CefTransitionType.ForwardBackFlag) != 0 || navigation_type == CefNavigationType.BackForward)
            {
                return true;
            }

            return base.OnBeforeNavigation(browser, frame, request, navigation_type, isRedirect);
        }
        */
        protected override void OnRenderThreadCreated(CefListValue extraInfo)
        {
            LogManager.WriteLog("-> OnRenderThreadCreated!");
            base.OnRenderThreadCreated(extraInfo);
        }

        protected override void OnWebKitInitialized()
        {
            LogManager.WriteLog("-> OnWebKitInitialized!");
            base.OnWebKitInitialized();
        }

        protected override void OnBrowserDestroyed(CefBrowser browser)
        {
            LogManager.WriteLog("-> OnBrowserDestroyed!");
            base.OnBrowserDestroyed(browser);
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            LogManager.WriteLog("-> GetLoadHandler!");
            return base.GetLoadHandler();
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            LogManager.WriteLog("-> OnContextReleased!");
            base.OnContextReleased(browser, frame, context);
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            LogManager.WriteLog("-> OnUncaughtException!");
            base.OnUncaughtException(browser, frame, context, exception, stackTrace);
        }

        protected override void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDomNode node)
        {
            LogManager.WriteLog("-> OnFocusedNodeChanged!");
            base.OnFocusedNodeChanged(browser, frame, node);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        protected override void OnBrowserCreated(CefBrowser browser, CefDictionaryValue extraInfo)
        {
            base.OnBrowserCreated(browser, extraInfo);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
