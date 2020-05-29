#pragma once
#include "../core/core.hpp"
#include "../wrapper/Main.hpp"

namespace rh2::hooking::input
{
    using KeyboardCallback = void (*)(uint32_t keyCode,
                                      uint16_t repeats,
                                      uint8_t  scanCode,
                                      bool     isExtended,
                                      bool     isWithAlt,
                                      bool     wasDownBefore,
                                      bool     isUpNow);

    bool InitializeHook();

    bool RemoveHook();
} 