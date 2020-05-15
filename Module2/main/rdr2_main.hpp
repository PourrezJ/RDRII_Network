#pragma once

#pragma unmanaged
#include <Windows.h>
#include <iostream>
#include <fstream>
#include <stdio.h> 
#include <algorithm>
#include <vector>
#include <Psapi.h>
#include <map>
#include <cstdint>
#include <cmath>

#include <thread>
#include <chrono>
#include <atomic>

#include <MinHook/MinHook.h>

#include "../types.h"
#include "../logs/unManagedLog.hpp"
#include "../memory/memory-location.hpp"
#include "../invoker/invoker.hpp"
#include "../memory/memory.hpp"
#include "../managed/Main.hpp"
#include "../hooking/natives_hook.hpp"
#include "../managed/Main.hpp"

namespace rdrn_module
{
	static std::atomic_bool g_unloading = false;
	static std::atomic_bool g_scriptCanBeStarted = false;

	static hMod g_module;

	bool initialize(hMod module);

	bool unload();

	void onTick();
}