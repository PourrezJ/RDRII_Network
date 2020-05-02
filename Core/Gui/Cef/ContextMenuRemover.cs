using Xilium.CefGlue;

namespace RDRN_Core.Gui.Cef
{
    internal class ContextMenuRemover : CefContextMenuHandler
    {
        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            model.Clear();
        }
    }
}
