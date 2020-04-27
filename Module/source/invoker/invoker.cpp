#pragma unmanaged

#include "invoker.hpp"

#include "../core/core.hpp"
#include "../hash_to_address_table.hpp"
#include <exception>
#include <Windows.h>
#include <unordered_map>

namespace rh2
{
    std::unordered_map<u64, NativeHandler> g_handlerCache;
    NativeHash                             g_commandHash = 0x0;
    rage::scrThread::Info                  g_callInfo;

    void Invoker::_NativeInit(NativeHash hash)
    {
        g_commandHash = hash;
        g_callInfo.Reset();
    }

    void Invoker::_NativePush(u64 val)
    {
        g_callInfo.Push(val);
    }

    DECLSPEC_NOINLINE u64* Invoker::_NativeCall()
    {
        if (auto handler = GetCommandHandler(g_commandHash))
        {
            __try
            {
                handler(&g_callInfo);
            }
            __except (EXCEPTION_EXECUTE_HANDLER)
            {
            }
        }

        return reinterpret_cast<u64*>(g_callInfo.GetResultPointer());
    }

    static uintptr_t base_address; 

    NativeHandler Invoker::GetCommandHandler(NativeHash command)
    {
        if (base_address == NULL)
            base_address = (uintptr_t)GetModuleHandleA(0);

        auto it = nativehash_to_address_table.find(command);
        if (it != nativehash_to_address_table.end())
        {
            if (it->first == command)
                return (NativeHandler)(base_address + it->second);
        }
        return 0;
    }
}
