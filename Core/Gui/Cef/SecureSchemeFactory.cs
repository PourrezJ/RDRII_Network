using RDRN_Module;
using System;
using System.IO;
using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class SecureSchemeFactory : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            Browser father = null;

            //LogManager.CefLog("-> Entering request w/ schemeName " + schemeName);

            try
            {
                father = CefUtil.GetBrowserFromCef(browser);

                if (father == null || father._localMode)
                {
                    LogManager.WriteLog(LogLevel.Trace, "-> [Local mode] Uri: " + request.Url);
                    var uri = new Uri(request.Url);
                    var path = Main.RDRNetworkPath + "resources\\";
                    var requestedFile = path + uri.Host + uri.LocalPath.Replace("/", "\\");

                    LogManager.WriteLog(LogLevel.Trace, "-> Loading: " + requestedFile);

                    if (File.Exists(requestedFile))
                        return SecureCefResourceHandler.FromFilePath(requestedFile,
                            MimeType.GetMimeType(Path.GetExtension(requestedFile)));
                    LogManager.WriteLog(LogLevel.Error, "-> Error: File does not exist!");
                    browser.StopLoad();
                    return SecureCefResourceHandler.FromString("404", ".txt");
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "CEF SCHEME HANDLING");
                browser?.StopLoad();
                return SecureCefResourceHandler.FromString("error", ".txt");
            }

            return null;
        }
    }
}
