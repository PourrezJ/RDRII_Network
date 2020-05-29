#define _CRT_SECURE_NO_WARNINGS
#pragma unmanaged
#include "core.hpp"
#include "logs.hpp"
#include "script.hpp"

#include "../hooking/detour-hook.hpp"
#include "../hooking/command-hook.hpp"
#include "../hooking/input-hook.hpp"
#include "../invoker/invoker.hpp"
#include "../logger/log-mgr.hpp"

#include <MinHook/MinHook.h>
#include <fmt/format.h>
#include <filesystem>
#include <shared_mutex>

#include "../wrapper/Main.hpp"
#include "../wrapper/ManagedGlobals.h"
namespace rh2
{
    std::atomic_bool g_unloading = false;
    std::shared_mutex g_scriptMutex;

    std::unique_ptr<hooking::CommandHook> g_waitHook;

    Fiber g_gameFiber;
    hMod g_module;
    Script g_clrScript = nullptr;

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
        
        logs::g_hLog->log("Game window found");
        
        logs::g_hLog->log("Initializing Minhook");
        auto st = MH_Initialize();
        if (st != MH_OK)
        {
            logs::g_hLog->log("Minhook failed to initialize {} ({})", MH_StatusToString(st), st);
            return false;
        }
        logs::g_hLog->log("Minhook initialized");

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

        scriptRegister(rh2::ClrInit);

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

            if (&g_clrScript != nullptr)
                g_clrScript.update();
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

    void scriptRegister(void (*LP_SCRIPT_MAIN)())
    {
        Script& script = rh2::Script(LP_SCRIPT_MAIN);
        g_clrScript = script;
    }

    void ScriptWait(const std::chrono::high_resolution_clock::duration& duration)
    {
        Script* script = &g_clrScript;

        if (script != nullptr)
            script->wait(duration);
    }


    Fiber GetGameFiber()
    {
        return g_gameFiber;
    }

    hMod GetModule() {
        return g_module;
    }
}
