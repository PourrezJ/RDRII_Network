#pragma once

#include "Main.hpp"
#include "ManagedLog.h"

#pragma managed
bool ManagedInit()
{
    System::Console::WriteLine("CLR INIT OK!:");
    return true;
}

void ManagedTick()
{
    System::Console::WriteLine("Tick");
}

#pragma unmanaged
#include "../core/core.hpp"
#include "../core/logs.hpp"

bool sGameReloaded = false;
PVOID sMainFib = nullptr;
PVOID sScriptFib = nullptr;

namespace rh2
{
    void ClrInit()
    {
        rh2::logs::g_hLog->log("Clr Init");

        sGameReloaded = true;
        sMainFib = GetCurrentFiber();

        if (sScriptFib == nullptr)
        {
            const LPFIBER_START_ROUTINE FiberMain = [](LPVOID lpFiberParameter)
            {
                while (ManagedInit())
                {
                    sGameReloaded = false;
                    while (!sGameReloaded)
                    {
                        ManagedTick();
                        SwitchToFiber(sMainFib);
                    }
                }
            };
            sScriptFib = CreateFiber(0, FiberMain, nullptr);
        }

        while (true)
        {
            rh2::ScriptWait(0);
            SwitchToFiber(sScriptFib);
        }
    }
}




