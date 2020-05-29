#pragma unmanaged

#include "invoker.hpp"

#include "../hash_to_address_table.hpp"
#include "../memory/patternscan.hpp"
#include "../core/core.hpp"

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

    static uintptr_t native;

    NativeHandler Invoker::GetCommandHandler(NativeHash native_hash)
    {
        if (!native)
            native = find_signature(0, "\x0F\xB6\xC1\x48\x8D\x15\x00\x00\x00\x00\x4C\x8B\xC9", "xxxxxx????xxx");

        static auto get_native_address = reinterpret_cast<uintptr_t(*)(uint64_t)>(native);
        return (NativeHandler)(get_native_address(native_hash));
    }

    static uintptr_t find_signature(const char* module, const char* pattern_, const char* mask) {
        const auto compare = [](const uint8_t* data, const uint8_t* pattern, const char* mask_) {
            for (; *mask_; ++mask_, ++data, ++pattern)
                if (*mask_ == 'x' && *data != *pattern)
                    return false;

            return (*mask_) == 0;
        };


        MODULEINFO module_info = {};
        GetModuleInformation(GetCurrentProcess(), GetModuleHandleA(module), &module_info, sizeof MODULEINFO);

        auto module_start = uintptr_t(module_info.lpBaseOfDll);
        const uint8_t* pattern = reinterpret_cast<const uint8_t*>(pattern_);
        for (size_t i = 0; i < module_info.SizeOfImage; i++)
            if (compare(reinterpret_cast<uint8_t*>(module_start + i), pattern, mask))
                return module_start + i;

        return 0;
    }
}
