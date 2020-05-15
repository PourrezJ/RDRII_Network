
#pragma managed
#include "Main.hpp"




bool ManagedInit()
{
	if (!System::Object::ReferenceEquals(ScriptHook::Domain, nullptr))
	{
		RDRN_Module::ScriptDomain::Unload(ScriptHook::Domain);
	}

	auto location = System::Reflection::Assembly::GetExecutingAssembly()->Location;

	ScriptHook::Domain = RDRN_Module::ScriptDomain::Load(System::IO::Path::Combine(System::IO::Path::GetDirectoryName(location) + "\\Scripts"));

	if (!System::Object::ReferenceEquals(ScriptHook::Domain, nullptr))
	{
		ScriptHook::Domain->Start();

		return true;
	}

	return false;
}
void ManagedTick()
{
	ScriptHook::Domain->DoTick();
}
void ManagedKeyboardMessage(int key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
{
	if (System::Object::ReferenceEquals(ScriptHook::Domain, nullptr))
	{
		return;
	}

	ScriptHook::Domain->DoKeyboardMessage(static_cast<System::Windows::Forms::Keys>(key), status, statusCtrl, statusShift, statusAlt);
}

#pragma unmanaged
#include "../invoker/invoker.hpp"
#pragma managed

System::UInt64* RDRN_Module::InvokeManaged(RDRN_Module::Native::Hash hash, ... array<System::UInt64>^ arguments)
{



	//invoke<rdrn_module::u64>(hash);
	return 0;
}

#pragma unmanaged
#include "../utils/fiber.hpp"
#include <WinBase.h>

bool sGameReloaded = false;
PVOID sMainFib = nullptr;
PVOID sScriptFib = nullptr;


static void ScriptMain()
{
	sGameReloaded = true;
	sMainFib = GetCurrentFiber();

	if (sScriptFib == nullptr)
	{
		const LPFIBER_START_ROUTINE FiberMain = [](LPVOID lpFiberParameter)
		{
			while (ManagedInit())
			{
				sGameReloaded = false;
				while (!sGameReloaded)
				{
					ManagedTick();
					SwitchToFiber(sMainFib);
				}
			}
		};
		sScriptFib = CreateFiber(0, FiberMain, nullptr);
	}

	while (true)
	{
		//fiber::
		SwitchToFiber(sScriptFib);
	}
}
