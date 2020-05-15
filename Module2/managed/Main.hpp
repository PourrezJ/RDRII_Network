#pragma once

#include "ScriptDomain.hpp"

#pragma managed
#include "NativeHashes.hpp"

#include <Windows.h>


ref struct ScriptHook
{
	static RDRN_Module::ScriptDomain^ Domain = nullptr;
};


bool ManagedInit();

void ManagedTick();

void ManagedKeyboardMessage(int key, bool status, bool statusCtrl, bool statusShift, bool statusAlt);

namespace RDRN_Module
{
	System::UInt64* InvokeManaged(RDRN_Module::Native::Hash hash, ... array<System::UInt64>^ arguments);
}
