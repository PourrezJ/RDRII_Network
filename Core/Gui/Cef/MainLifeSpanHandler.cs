using RDRN_Module;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class MainLifeSpanHandler : CefLifeSpanHandler
    {
        private MainCefClient bClient;


        internal MainLifeSpanHandler(MainCefClient bc)
        {
            LogManager.WriteLog("-> MainLifeSpanHandler");
            this.bClient = bc;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            LogManager.WriteLog("-> OnAfterCreated");
            base.OnAfterCreated(browser);
            this.bClient.Created(browser);
        }

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
        {
            LogManager.WriteLog("-> OnBeforePopup");
            Browser father = CefUtil.GetBrowserFromCef(browser);
            father.GoToPage(targetUrl);
            return base.OnBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref extraInfo, ref noJavascriptAccess);
        }
    }
}
