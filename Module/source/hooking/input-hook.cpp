#include "input-hook.hpp"
#include "../wrapper/Input.h"
#include <windows.h>
#include <set>
#include <exception>

namespace rh2::hooking::input
{
    WNDPROC                    g_oWndProc     = nullptr;
    HWND                       g_windowHandle = 0;

    LRESULT APIENTRY WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
        case WM_KEYDOWN:
        case WM_KEYUP:
        case WM_SYSKEYDOWN:
        case WM_SYSKEYUP: 
                 ManagedScriptKeyboardMessage(
                static_cast<uint32_t>(wParam), lParam & 0xFFFF, (lParam >> 16) & 0xFF,
                (lParam >> 24) & 1, (msg == WM_SYSKEYDOWN || msg == WM_SYSKEYUP),
                (lParam >> 30) & 1, (msg == WM_SYSKEYUP || msg == WM_KEYUP));
            break;
        default: break;
        }

        return CallWindowProcW(g_oWndProc, hwnd, msg, wParam, lParam);
    }

    bool InitializeHook()
    {
        g_windowHandle = FindWindowA("sgaWindow", "Red Dead Redemption 2");

        g_oWndProc = reinterpret_cast<WNDPROC>(
            SetWindowLongPtr(g_windowHandle, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(WndProc)));

        return g_oWndProc != nullptr;
    }

    bool RemoveHook()
    {
        SetWindowLongPtr(g_windowHandle, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(g_oWndProc));

        return true;
    }
} // namespace rh2::hooking::input
