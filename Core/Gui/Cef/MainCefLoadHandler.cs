﻿using RDRN_Module;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class MainCefLoadHandler : CefLoadHandler
    {
        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            // A single CefBrowser instance can handle multiple requests
            //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
            //if (frame.IsMain)
            {
                LogManager.WriteLog("-> Start: " + browser.GetMainFrame().Url);
            }
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            //if (frame.IsMain)
            {
                LogManager.WriteLog($"-> End: {browser.GetMainFrame().Url}, {httpStatusCode}");
            }
        }
    }
}
