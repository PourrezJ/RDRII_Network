#pragma once
#include "../core/core.hpp"

namespace rh2
{
    void ClrInit();
    void ScriptKeyboardMessage(DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow);
}

bool ManagedInit();

void ManagedTick();
