#pragma once
#include "../main/rdr2_main.hpp"
#pragma comment(lib, "winmm.lib")
#include <timeapi.h>
namespace fiber
{
	extern void wait_for(DWORD ms);
	extern void on_tick();
}