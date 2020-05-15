#include "command-hook.hpp"

namespace rh2::hooking
{
    CommandHook::CommandHook(const NativeHash native, NativeHandler detour) :
        DetourHook(rh2::Invoker::GetCommandHandler(native), detour)
    {
    }

    void CommandHook::orig(rage::scrThread::Info* info)
    {
        DetourHook::orig<void>(info);
    }
} // namespace rh2::hooking
