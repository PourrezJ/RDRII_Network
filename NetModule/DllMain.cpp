#pragma managed
#include "LogManager.h"

using namespace System;
using namespace System::Reflection;

delegate void KeyboardMethodDelegate(System::Windows::Forms::Keys key, bool status, bool statusCtrl, bool statusShift, bool statusAlt);


ref struct LoaderData
{
	static Assembly^ Assembly;
	static Func<bool>^ InitMethods;
	static Action^ Tick;

};

void ManagedPreInit()
{
	String^ directory = System::IO::Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location);

	try 
	{
		LoaderData::Assembly = Assembly::LoadFrom(IO::Path::Combine(directory, "Scripts\\RDRN_Core.dll"));

		System::Type^ main = LoaderData::Assembly->GetType("RDRN_Core.Main");

		LoaderData::InitMethods = safe_cast<Func<bool>^>(main->GetMethod("OnInit", BindingFlags::Public | BindingFlags::Static)->CreateDelegate(Func<bool>::typeid));
		LoaderData::Tick = safe_cast<Action^>(main->GetMethod("OnTick", BindingFlags::Public | BindingFlags::Static)->CreateDelegate(Action::typeid));

		main->GetMethod("OnPreInit", BindingFlags::Public | BindingFlags::Static)->Invoke(nullptr, gcnew array<Object^> { directory });
	}
	catch (System::Exception^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Failed to load RDRN_Core: {0}", ex->ToString());
		if (ex->InnerException != nullptr) {
			RDRN_Module::LogManager::WriteLog("*** InnerException: {0}", ex->InnerException->ToString());
		}
		return;
	}
}

void ManagedInit()
{
	LoaderData::InitMethods();
}

void ManagedTick()
{
	LoaderData::Tick();
}

void ManagedKeyboardMessage(unsigned long key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
{

}


#pragma unmanaged
#include <Main.h>
#include <Windows.h>
#include <stdio.h>

bool sGameReloaded = false;
PVOID sMainFib = nullptr;
PVOID sScriptFib = nullptr;

static void ScriptMain()
{
	sGameReloaded = true;

	// ScriptHookV already turned the current thread into a fiber, so we can safely retrieve it.
	sMainFib = GetCurrentFiber();

	// Check if our CLR fiber already exists. It should be created only once for the entire lifetime of the game process.
	if (sScriptFib == nullptr)
	{
		const LPFIBER_START_ROUTINE FiberMain = [](LPVOID lpFiberParameter) {
			while (true)
			{
				ManagedInit();

				sGameReloaded = false;

				// If the game is reloaded, ScriptHookV will call the script main function again.
				// This will set the global 'sGameReloaded' variable to 'true' and on the next fiber switch to our CLR fiber, run into this condition, therefore exiting the inner loop and re-initialize.
				while (!sGameReloaded)
				{
					ManagedTick();

					// Switch back to main script fiber used by ScriptHookV.
					// Code continues from here the next time the loop below switches back to our CLR fiber.
					SwitchToFiber(sMainFib);
				}
			}
		};

		// Create our own fiber for the common language runtime, aka CLR, once.
		// This is done because ScriptHookV switches its internal fibers sometimes, which would corrupt the CLR stack.
		sScriptFib = CreateFiber(0, FiberMain, nullptr);
	}

	while (true)
	{
		// Yield execution and give it back to ScriptHookV.
		scriptWait(0);

		// Switch to our CLR fiber and wait for it to switch back.
		SwitchToFiber(sScriptFib);
	}
}

static void ScriptKeyboardMessage(DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow)
{
	ManagedKeyboardMessage(key, isUpNow == FALSE, (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0, (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0, isWithAlt != FALSE);
}

BOOL APIENTRY DllMain(HMODULE hInstance, DWORD reason, LPVOID lpReserved)
{
	if (reason == DLL_PROCESS_ATTACH) 
	{
		if (AllocConsole())
		{
			freopen("CONIN$", "r", stdin);
			freopen("CONOUT$", "w", stdout);
			freopen("CONOUT$", "w", stderr);
			SetConsoleTitleA("Red Dead Redemption II: Network");
		}

		DisableThreadLibraryCalls(hInstance);
		scriptRegister(hInstance, &ScriptMain);
		keyboardHandlerRegister(&ScriptKeyboardMessage);

		ManagedPreInit();

	}
	else if (reason == DLL_PROCESS_DETACH) 
	{
		
	}

	return TRUE;
}
