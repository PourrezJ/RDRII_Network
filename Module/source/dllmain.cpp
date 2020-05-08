// dllmain.cpp : Defines the entry point for the DLL application.

#pragma unmanaged
#include "core/core.hpp"


DWORD WINAPI ControlThread(const LPVOID param)
{
    if (!rh2::Init(param))
    {
        return 1;
    }

    return 0;
}

BOOL APIENTRY DllMain(const HMODULE hModule, const DWORD reasonForCall, const LPVOID lpReserved)
{
    if (reasonForCall == DLL_PROCESS_ATTACH)
    {
        CreateThread(NULL, NULL, ControlThread, hModule, 0, 0);
    }
    return TRUE;
}
