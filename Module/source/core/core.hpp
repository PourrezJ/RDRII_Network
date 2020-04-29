#pragma once

#include "../util/fiber.hpp"
#include "../memory/memory-location.hpp"
#include "../rage/scrProgram.hpp"
#include "../types.hpp"

#include <chrono>

namespace rh2
{
    bool Init(hMod module);

    void Unload();

    Fiber GetGameFiber();

    MemoryLocation GetPatchVectorResults();

    MemoryLocation Get_s_CommandHash();

    MemoryLocation Get_rage__scrThread__GetCmdFromHash();

    DWORD WINAPI CleanupThread(LPVOID lparam);
    template<typename T>
    T* GetGlobalPtr(uint32_t index)
    {
        if (!rage::scrProgram::sm_Globals)
            return nullptr;

        return *reinterpret_cast<T**>(
            &rage::scrProgram::sm_Globals[index >> 18 & 0x3F][index & 0x3FFFF]);
    }
} // namespace rh2
