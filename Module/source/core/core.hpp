#pragma once

#include "../util/fiber.hpp"
#include "../memory/memory-location.hpp"
#include "../rage/scrProgram.hpp"
#include "../types.hpp"

#include <chrono>
#include <Psapi.h>

namespace rh2
{
    bool Init(hMod module);

    void Unload();

    void scriptRegister(void(*LP_SCRIPT_MAIN)());
    void ScriptWait(uint32_t duration);

    Fiber GetGameFiber();

    hMod GetModule();

    DWORD WINAPI CleanupThread(LPVOID lparam);
    template<typename T>
    T* GetGlobalPtr(uint32_t index)
    {
        if (!rage::scrProgram::sm_Globals)
            return nullptr;

        return *reinterpret_cast<T**>(
            &rage::scrProgram::sm_Globals[index >> 18 & 0x3F][index & 0x3FFFF]);
    }
}