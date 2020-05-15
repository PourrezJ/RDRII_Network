#include "natives_hook.hpp"

#include "../types.h"

namespace hooks
{
	namespace original
	{
		start_new_script_wnhaa_fn o_start_new_script_wnhaa;
		shutdownLoadingScreen_hook_fn o_shutdownLoadingScreen;
		show_loading_screen_native_fn o_show_loading_screen_native;
		wait_hook_fn o_wait;
	}

	int __fastcall shutdownLoadingScreen_hook()
	{
		rdrn_module::g_scriptCanBeStarted = true;

		return original::o_shutdownLoadingScreen();
	}

	void __fastcall wait_hook(Context* ctx)
	{
		if (!rdrn_module::g_scriptCanBeStarted)
		{
			original::o_wait(ctx);
			return;
		}

		fiber::on_tick();

		original::o_wait(ctx);
	}

	void __fastcall start_new_script_wnhaa(uintptr_t a1, DWORD a2) {
		//printf("script: %p %p\n", a1, a2);
		original::o_start_new_script_wnhaa(a1, a2);
	}

	void __fastcall show_loading_screen_native_hook(DWORD a1, DWORD a2, DWORD a3, const char* gamemodeName, const char* title, const char* subtitle) {
		printf("show_loading_screen_native_hook: %p %p\n", a1, a2);
		original::o_show_loading_screen_native(a1, a2, a3, gamemodeName, title, subtitle);
	}

	void initialize(MH_STATUS status)
	{
		auto shutdownLoadingScreen_native = memory::find_signature(0, "\x8a\x05\x00\x00\x00\x00\x84\xc0\x75\x00\xc6\x05", "xx????xxx?xx");
		status = MH_CreateHook((PVOID)shutdownLoadingScreen_native, shutdownLoadingScreen_hook, reinterpret_cast<void**>(&original::o_shutdownLoadingScreen));
		printf("create_status : %s\n", std::string(MH_StatusToString(status)).c_str());
		status = MH_EnableHook((PVOID)shutdownLoadingScreen_native);
		
		auto wait_native = get_handler(0x4EDE34FBADD967A6);
		status = MH_CreateHook((PVOID)wait_native, wait_hook, reinterpret_cast<void**>(&original::o_wait));
		printf("create_status : %s\n", std::string(MH_StatusToString(status)).c_str());
		status = MH_EnableHook((PVOID)wait_native);
			

		auto start_new_script_wnhaa_native = (void*)get_handler(0xE81651AD79516E48);
		status = MH_CreateHook((PVOID)start_new_script_wnhaa_native, start_new_script_wnhaa, reinterpret_cast<void**>(&original::o_start_new_script_wnhaa));
		status = MH_EnableHook((PVOID)start_new_script_wnhaa_native);
		printf("create_status : %s\n", std::string(MH_StatusToString(status)).c_str());

		auto show_loading_screen_native = (void*)get_handler(0x1E5B70E53DB661E5);
		status = MH_CreateHook((PVOID)show_loading_screen_native, show_loading_screen_native_hook, reinterpret_cast<void**>(&original::o_show_loading_screen_native));
		status = MH_EnableHook((PVOID)show_loading_screen_native);
		printf("create_status : %s\n", std::string(MH_StatusToString(status)).c_str());

	}
}
