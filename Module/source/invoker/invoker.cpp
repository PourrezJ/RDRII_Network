#pragma unmanaged

#include "invoker.hpp"

#include "../hash_to_address_table.hpp"
#include "../memory/patternscan.hpp"

namespace rh2
{
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
            handler(&g_callInfo);
        }

        return reinterpret_cast<u64*>(g_callInfo.GetResultPointer());
    }

    NativeHandler Invoker::GetCommandHandler(NativeHash native_hash)
    {        /*
        printf("Address: %p\n", native_hash);

        uint16_t native = rh2::PatternScan("\x0F\xB6\xC1\x48\x8D\x15\x00\x00\x00\x00\x4C\x8B\xC9", "xxxxxx????xxx");
        auto get_native_address = reinterpret_cast<uintptr_t(*)(uint64_t)>(native);
        
        //auto get_native_address = reinterpret_cast<uintptr_t(*)(uint64_t)>((uintptr_t(GetModuleHandleW(0))) + 0x2a4fcc8);
        return (NativeHandler)(get_native_address(native_hash));*/

        auto base_address = (uintptr_t)GetModuleHandleA(0);

        auto it = nativehash_to_address_table.find(native_hash);
        if (it != nativehash_to_address_table.end())
        {
            if (it->first == native_hash)
                return (NativeHandler)(base_address + it->second);
        }
        else
            printf("Can't find address: %p\n", native_hash);
        return 0;
    }
}
