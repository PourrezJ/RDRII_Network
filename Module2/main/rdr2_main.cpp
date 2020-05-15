#pragma unmanaged
#include "rdr2_main.hpp"

namespace rdrn_module
{
	//ped pool 48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 8B 15 ? ? ? ? 48 8B F1 48 83 C1 10 33 FF

	bool initialize(hMod module) {

		g_module = module;

		if (AllocConsole()) {
			freopen("CONIN$", "r", stdin);
			freopen("CONOUT$", "w", stdout);
			freopen("CONOUT$", "w", stderr);
		}

		rdrn_module::UnmanagedLogWrite("RDRII Network loading!");

		using namespace std::chrono;
		using namespace std::chrono_literals;

		rdrn_module::UnmanagedLogWrite("Red Dead Redemption 2 window waiting...");

		auto timeout = high_resolution_clock::now() + 20s;
		while (!FindWindowA("sgaWindow", "Red Dead Redemption 2") &&
			high_resolution_clock::now() < timeout)
		{
			std::this_thread::sleep_for(100ms);
		}
		std::this_thread::sleep_for(2s);

		if (high_resolution_clock::now() >= timeout)
		{
			rdrn_module::UnmanagedLogWrite("Timed out");
			return false;
		}

		rdrn_module::UnmanagedLogWrite("Initializing Minhook");
		auto st = MH_Initialize();
		if (st != MH_OK)
		{
			rdrn_module::UnmanagedLogWrite("Minhook failed to initialize {} ({})", MH_StatusToString(st), st);
			return false;
		}
		rdrn_module::UnmanagedLogWrite("Minhook initialized");

		std::this_thread::sleep_for(20s);
		rdrn_module::UnmanagedLogWrite("Initializing native hooks");
		hooks::initialize(st);
		rdrn_module::UnmanagedLogWrite("Natives initialized");

		return true;
	}

	bool unload() 
	{
		if (g_unloading)
			return false;
		rdrn_module::UnmanagedLogWrite("Uninitializing Minhook");

		g_unloading = true;

		auto st = MH_Uninitialize();
		if (st != MH_OK)
		{
			rdrn_module::UnmanagedLogWrite("Failed to unitialized Minhook {} ({})", MH_StatusToString(st), st);
			return false;
		}
		rdrn_module::UnmanagedLogWrite("Minhook uninitialized");
		
		FreeConsole();
		FreeLibraryAndExitThread(GetModuleHandleA(0), 0);
		return true;
	}

	static bool init;
	void onTick() 
	{
		if (!init) {
			init = true;
			printf("init");
			ManagedInit();
		}
		ManagedTick();
		//invoke(0xFC179D7E8886DADF);
	}
}

