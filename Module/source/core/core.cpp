#define _CRT_SECURE_NO_WARNINGS
#pragma unmanaged
#include "core.hpp"

#include "../memory/patternscan.hpp"
#include "../hooking/detour-hook.hpp"
#include "../hooking/command-hook.hpp"
#include "../hooking/input-hook.hpp"
#include "../invoker/invoker.hpp"
#include "../logger/log-mgr.hpp"
#include "logs.hpp"

#include <memory>
#include <MinHook/MinHook.h>
#include <thread>
#include <chrono>
#include <fmt/format.h>
#include <unordered_map>
#include <filesystem>
#include <atomic>
#include <unordered_set>

#include <shared_mutex>
#include "../wrapper/Main.hpp"
#include "../wrapper/ManagedGlobals.h"

namespace rh2
{
    std::atomic_bool g_unloading = false;
    std::shared_mutex g_scriptMutex;


    //MemoryLocation g_PatchVectorResults;
   // MemoryLocation g_s_CommandHash;
    //MemoryLocation g_rage__scrThread__GetCmdFromHash;
    //MemoryLocation g_rage__scrProgram__sm_Globals;

    MemoryLocation g_loadingScreen;

    std::unique_ptr<hooking::CommandHook> g_waitHook;

    Fiber g_gameFiber;
    hMod g_module;

    bool InitializeCommandHooks();
    void CreateLogs();

    bool Init(hMod module)
    {
        if (AllocConsole())
        {
            freopen("CONIN$", "r", stdin);
            freopen("CONOUT$", "w", stdout);
            freopen("CONOUT$", "w", stderr);
            SetConsoleTitle("Red Dead Redemption II: Network");
        }

        using namespace literals;
        using namespace std::chrono;
        using namespace std::chrono_literals;

        g_module = module;

        CreateLogs();

        logs::g_hLog->log("Initializing Red Dead Redemption II Network");

        logs::g_hLog->log("Waiting for game window");

        // Wait for the game window, otherwise we can't do much
        
        auto timeout = high_resolution_clock::now() + 20s;
        while (!FindWindowA("sgaWindow", "Red Dead Redemption 2") &&
               high_resolution_clock::now() < timeout)
        {
            std::this_thread::sleep_for(100ms);
        }
        std::this_thread::sleep_for(10s);
        


        // Check if waiting for the window timed out
        /*
        if (high_resolution_clock::now() >= timeout)
        {
            logs::g_hLog->fatal("Timed out");
            return false;
        }*/
        logs::g_hLog->log("Game window found");
        /*
        logs::g_hLog->log("Searching patterns");

        // Find sigs
        MemoryLocation loc;

        // PatchVectorResults
        if (loc = "8B 41 18 4C 8B C1 85 C0"_Scan)
            g_PatchVectorResults = loc;
        else
            return false;

        // rage::scrThread::GetCmdFromhash
        if (loc = "E8 ? ? ? ? 8B 9C F5 ? ? ? ?"_Scan)
        {
            g_rage__scrThread__GetCmdFromHash = loc.get_call();
            s_CommandHash = g_s_CommandHash = loc.get_call().add(3).get_lea();
        }
        else
            return false;

        if (loc = "4C 8D 05 ? ? ? ? 4D 8B 08 4D 85 C9 74 11")
            rage::scrProgram::sm_Globals = g_rage__scrProgram__sm_Globals = loc.get_lea();
        else
            false;

        logs::g_hLog->log("Patterns found");
        */

        MemoryLocation loc;

        if (loc = "8A 05 ? ? ? ? 84 C0 75 ? C6 05 ? ? ? ? ?"_Scan)
        {
            g_loadingScreen = loc.get_call();

        }


        logs::g_hLog->log("Initializing Minhook");
        auto st = MH_Initialize();
        if (st != MH_OK)
        {
            logs::g_hLog->log("Minhook failed to initialize {} ({})", MH_StatusToString(st), st);
            return false;
        }
        logs::g_hLog->log("Minhook initialized");

        /*
        logs::g_hLog->log("Waiting for natives");
        while (!(*s_CommandHash))
        {
            std::this_thread::sleep_for(100ms);
        }
        logs::g_hLog->log("Natives loaded");
        */
        
        logs::g_hLog->log("Initializing input hook");
        if (!hooking::input::InitializeHook())
        {
            return false;
        }
        logs::g_hLog->log("Input hook initialized");

        logs::g_hLog->log("Initializing native hooks");
        
        if (!InitializeCommandHooks())
        {
            return false;
        }
        logs::g_hLog->log("Natives initialized");

        rh2::ClrInit();

        return true;
    }

    void Unload()
    {
        CreateThread(nullptr, THREAD_ALL_ACCESS, CleanupThread, nullptr, NULL, nullptr);    
    }

    DWORD WINAPI CleanupThread(LPVOID lparam) {
        using namespace std::chrono_literals;

        if (g_unloading)
            return false;
        g_unloading = true;

        logs::g_hLog->log("Removing input hook");
        if (!hooking::input::RemoveHook())
        {
            logs::g_hLog->fatal("Failed to remove input hook");
            return false;
        }
        logs::g_hLog->log("Input hook removed");

        logs::g_hLog->log("Removing hooks");
        if (!hooking::DisableHooks())
        {
            logs::g_hLog->fatal("Failed to disable hooks");
            return false;
        }
        logs::g_hLog->log("Hooks disabled");
        if (!hooking::RemoveHooks())
        {
            logs::g_hLog->fatal("Failed to remove hooks");
            return false;
        }
        logs::g_hLog->log("Hooks removed");

        logs::g_hLog->log("Uninitializing Minhook");
        auto st = MH_Uninitialize();
        if (st != MH_OK)
        {
            logs::g_hLog->fatal("Failed to unitialized Minhook {} ({})", MH_StatusToString(st), st);
            return false;
        }
        logs::g_hLog->log("Minhook uninitialized");

        logs::g_hLog->log("Restoring memory");
        MemoryLocation::RestoreAllModifiedBytes();
        logs::g_hLog->log("Memory restored");

        logs::g_hLog->log("RDRII: Network unloaded");

        logging::LogMgr::DeleteAllLogs();
        FreeConsole();
        FreeLibraryAndExitThread(static_cast<HMODULE>(g_module), 0);
    }

    void MyWait(rage::scrThread::Info* info)
    {
        if (g_unloading)
            return g_waitHook->orig(info);

        if (!g_gameFiber)
        {
            if (!(g_gameFiber = Fiber::ConvertThreadToFiber()))
            {
                g_gameFiber = Fiber::CurrentFiber();
            }
        }

        // GET_HASH_OF_THIS_SCRIPT_NAME
        if (Invoker::Invoke<u32>(0xBC2C927F5C264960ull) == 0x27eb33d7u) // main
        {
            std::lock_guard _(g_scriptMutex);
            rh2::ClrTick();
        }
        
        g_waitHook->orig(info);
    }

    bool InitializeCommandHooks()
    {
        g_waitHook = std::make_unique<hooking::CommandHook>(
            0x4EDE34FBADD967A6ull, reinterpret_cast<NativeHandler>(MyWait));

        return                      //
            g_waitHook->enable() && //
            true;                   //
    }

    void CreateLogs()
    {
        std::filesystem::create_directories("RedHook2/logs");

        logs::g_hLog = logging::LogMgr::CreateLog<logging::GenericFileLogger>(
            "hook_log", "RedHook2/logs/hook.log");
    }

    Fiber GetGameFiber()
    {
        return g_gameFiber;
    }

    hMod GetModule() {
        return g_module;
    }

    /*
    MemoryLocation GetPatchVectorResults()
    {
        return g_PatchVectorResults;
    }

    MemoryLocation Get_s_CommandHash()
    {
        return g_s_CommandHash;
    }

    MemoryLocation Get_rage__scrThread__GetCmdFromHash()
    {
        return g_rage__scrThread__GetCmdFromHash;
    }*/

} // namespace rh2
