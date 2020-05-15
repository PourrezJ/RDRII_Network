#pragma once

#include <cstdint>

namespace rdrn_module::hooking::input
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

    void AddCallback(KeyboardCallback callback);

    void RemoveCallback(KeyboardCallback callback);
} // namespace rh2::hooking::input