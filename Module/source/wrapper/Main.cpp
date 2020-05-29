#pragma once

#include "Main.hpp"


#pragma managed
#include "ManagedLog.h"
#include "ScriptDomain.h"

ref struct ScriptHook
{
    static RDRN_Module::ScriptDomain^ Domain = nullptr;
};

bool ManagedInit()
{
    auto location = System::Reflection::Assembly::GetExecutingAssembly()->Location;

    ScriptHook::Domain = RDRN_Module::ScriptDomain::Load(System::IO::Path::Combine(System::IO::Path::GetDirectoryName(location), "Scripts"));

    if (!System::Object::ReferenceEquals(ScriptHook::Domain, nullptr))
    {
        ScriptHook::Domain->Start();

        return true;
    }

    return false;
}

void ManagedKeyboardMessage(int key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
{
    if (System::Object::ReferenceEquals(ScriptHook::Domain, nullptr))
    {
        return;
    }

    ScriptHook::Domain->DoKeyboardMessage(static_cast<System::Windows::Forms::Keys>(key), status, statusCtrl, statusShift, statusAlt);
}

void ManagedTick() {
    ScriptHook::Domain->DoTick();
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

        ManagedInit();

        while (!sGameReloaded)
        {
            ManagedTick();
            rh2::ScriptWait(std::chrono::milliseconds(0));
            //SwitchToFiber(sMainFib);
        }

        /*
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
        }*/
    }

    void ScriptKeyboardMessage(DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow)
    {
        ManagedKeyboardMessage(static_cast<int>(key), isUpNow == FALSE, (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0, (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0, isWithAlt != FALSE);
    }
}




