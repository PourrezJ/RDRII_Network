#include "input-hook.hpp"

#include <windows.h>
#include <set>
#include <exception>

namespace rh2::hooking::input
{

    HHOOK hook;

    // Forward declaration
    LRESULT CALLBACK msghook(int nCode, WPARAM wParam, LPARAM lParam);


    WNDPROC                    g_oWndProc     = nullptr;
    HWND                       g_windowHandle = 0;


    //msg: 513 clic gauche 516 clic droit
    LRESULT APIENTRY WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
        case WM_KEYDOWN:
        case WM_KEYUP:
        case WM_SYSKEYDOWN:
        case WM_SYSKEYUP: 
            
            rh2::ScriptKeyboardMessage(
                static_cast<uint32_t>(wParam), lParam & 0xFFFF, (lParam >> 16) & 0xFF,
                (lParam >> 24) & 1, (msg == WM_SYSKEYDOWN || msg == WM_SYSKEYUP),
                (lParam >> 30) & 1, (msg == WM_SYSKEYUP || msg == WM_KEYUP));
            break;
        }

        return CallWindowProcW(g_oWndProc, hwnd, msg, wParam, lParam);
    }

    bool InitializeHook()
    {
        
        g_windowHandle = FindWindowA("sgaWindow", "Red Dead Redemption 2");

        g_oWndProc = reinterpret_cast<WNDPROC>(
            SetWindowLongPtr(g_windowHandle, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(WndProc)));
        /*
        auto module = rh2::GetModule();
        
        hook = SetWindowsHookEx(
            WH_GETMESSAGE,
            (HOOKPROC)msghook,
            (HINSTANCE)module,
            0);

        if (hook != NULL)
        {
            //hWndServer = hWnd;
            printf("============================ hook ok :P =============================");
        } 
        */
    
        return g_oWndProc != nullptr;
    }

    bool RemoveHook()
    {
        SetWindowLongPtr(g_windowHandle, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(g_oWndProc));

        return true;
    }

    LRESULT CALLBACK msghook(int nCode, WPARAM wParam, LPARAM lParam)
    {
        return CallNextHookEx(hook, nCode, wParam, lParam);
    } 
} 
