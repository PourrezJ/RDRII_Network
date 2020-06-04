#define DLL_PORTING __declspec(dllexport)
#pragma once
#include "types.h"
#include "scrThread.h"

#include <utility>
#include <shared_mutex>

namespace rh2
{
    static std::shared_mutex g_invokeMutex;

    class DLL_PORTING Invoker
    {
    private:
        static void _NativeInit(NativeHash hash);

        static void _NativePush(u64 val);

        static u64* _NativeCall();



    public:


        static void NativeInit(NativeHash hash)
        {
            _NativeInit(hash);
        }

        template<typename T>
        static void NativePush(T val)
        {
            _NativePush(*reinterpret_cast<u64*>(&val));
        }

        template<typename T>
        static T NativeCall()
        {
            return *reinterpret_cast<T*>(_NativeCall());
        }

        static u64* NativeCall()
        {
            return _NativeCall();
        }

        template<typename R, typename... Args>
        static R Invoke(NativeHash hash, const Args&... args)
        {
            //std::lock_guard<std::shared_mutex> lock(g_invokeMutex);

            //g_invokeMutex.try_lock_shared();
            NativeInit(hash);
            (NativePush(args), ...);
            //g_invokeMutex.unlock_shared();
            return NativeCall<R>();
        }

        static NativeHandler GetCommandHandler(NativeHash command);
    };
    static uintptr_t find_signature(const char* module, const char* pattern_, const char* mask);
} 