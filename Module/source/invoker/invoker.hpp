#pragma once
#include "../types.hpp"

#include "../rage/scrThread.hpp"

#include <utility>

namespace rh2
{
    class Invoker
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
            NativeInit(hash);
            (NativePush(args), ...);
            return NativeCall<R>();
        }

        static NativeHandler GetCommandHandler(NativeHash command);
    };
} // namespace rh2
