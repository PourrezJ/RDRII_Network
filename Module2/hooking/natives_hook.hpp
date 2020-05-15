#pragma once
#include <Windows.h>
#include <string>
#include <Minhook/MinHook.h>

#include "../utils/fiber.hpp"
#include "../main/rdr2_main.hpp"

typedef int(__fastcall* shutdownLoadingScreen_hook_fn)();
typedef void(__fastcall* wait_hook_fn)(Context* ctx);
typedef void(__fastcall* start_new_script_wnhaa_fn)(uintptr_t a1, DWORD a2);
typedef void(__fastcall* show_loading_screen_native_fn)(DWORD a1, DWORD a2, DWORD a3, const char* gamemodeName, const char* title, const char* subtitle);

namespace hooks
{

	namespace original
	{
		extern start_new_script_wnhaa_fn o_start_new_script_wnhaa;
		extern shutdownLoadingScreen_hook_fn o_shutdownLoadingScreen;
		extern show_loading_screen_native_fn o_show_loading_screen_native;
		extern wait_hook_fn o_wait;
	}

	extern int __fastcall shutdownLoadingScreen_hook();
	extern void __fastcall wait_hook(Context* ctx);
	extern void __fastcall start_new_script_wnhaa(uintptr_t a1, DWORD a2);
	extern void __fastcall show_loading_screen_native_hook(DWORD a1, DWORD a2, DWORD a3, const char* gamemodeName, const char* title, const char* subtitle);

	extern void initialize(MH_STATUS status);

}