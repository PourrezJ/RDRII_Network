#include <vector>

#include <Script.h>
#include <Input.h>

#include <NativeHashes.h>

#include <Math/Matrix.hpp>
#include <Math/Quaternion.hpp>
#include <Math/Vector2.hpp>
#include <Math/Vector3.hpp>

#include <ManagedGlobals.h>
#include <ScriptDomain.h>
#include <Log.h>

#include <UnmanagedLog.h>

bool unload;

ref class ManagedEventSink
{
public:
	void OnUnhandledException(System::Object^ sender, System::UnhandledExceptionEventArgs^ args)
	{
		RDRN_Module::LogManager::WriteLog("*** Unhandled exception: {0}", args->ExceptionObject->ToString());
	}
};

void LoadScriptDomain()
{
	auto curDir = System::IO::Path::GetDirectoryName(System::Reflection::Assembly::GetExecutingAssembly()->Location);

	System::String^ strPath = System::Environment::GetEnvironmentVariable("PATH");
	if (!strPath->EndsWith(";")) {
		strPath += ";";
	}
	strPath += curDir + ";";
	strPath += curDir + "\\Scripts;";
	System::Environment::SetEnvironmentVariable("PATH", strPath);

	auto setup = gcnew System::AppDomainSetup();
	setup->ApplicationBase = System::IO::Path::GetFullPath(curDir + "\\Scripts");
	setup->ShadowCopyFiles = "false"; // !?
	setup->ShadowCopyDirectories = setup->ApplicationBase;

	RDRN_Module::LogManager::WriteLog("Creating AppDomain with base \"{0}\"", setup->ApplicationBase);

	auto appDomainName = "ScriptDomain_" + (curDir->GetHashCode() * System::Environment::TickCount).ToString("X");
	auto appDomainPermissions = gcnew System::Security::PermissionSet(System::Security::Permissions::PermissionState::Unrestricted);

	RDRN_Module::ManagedGlobals::g_appDomain = System::AppDomain::CreateDomain(appDomainName, nullptr, setup, appDomainPermissions);
	RDRN_Module::ManagedGlobals::g_appDomain->InitializeLifetimeService();

	RDRN_Module::LogManager::WriteLog("Created AppDomain \"{0}\"", RDRN_Module::ManagedGlobals::g_appDomain->FriendlyName);

	auto typeScriptDomain = RDRN_Module::ScriptDomain::typeid;
	try {
		RDRN_Module::ManagedGlobals::g_scriptDomain = static_cast<RDRN_Module::ScriptDomain^>(RDRN_Module::ManagedGlobals::g_appDomain->CreateInstanceFromAndUnwrap(typeScriptDomain->Assembly->Location, typeScriptDomain->FullName));
	} catch (System::Exception^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Failed to create ScriptDomain: {0}", ex->ToString());
		if (ex->InnerException != nullptr) {
			RDRN_Module::LogManager::WriteLog("*** InnerException: {0}", ex->InnerException->ToString());
		}
		return;
	} catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Failed to create ScriptDomain beacuse of unmanaged exception");
		return;
	}

	RDRN_Module::LogManager::WriteLog("Created ScriptDomain!");

	RDRN_Module::ManagedGlobals::g_scriptDomain->FindAllTypes();
}

static bool ManagedScriptInit(int scriptIndex, void* fiberMain, void* fiberScript)
{
	return RDRN_Module::ManagedGlobals::g_scriptDomain->ScriptInit(scriptIndex, fiberMain, fiberScript);
}

static bool ManagedScriptExists(int scriptIndex)
{
	if (RDRN_Module::ManagedGlobals::g_scriptDomain == nullptr) {
		return false;
	}
	return RDRN_Module::ManagedGlobals::g_scriptDomain->ScriptExists(scriptIndex);
}

static int ManagedScriptGetWaitTime(int scriptIndex)
{
	return RDRN_Module::ManagedGlobals::g_scriptDomain->ScriptGetWaitTime(scriptIndex);
}

static void ManagedScriptResetWaitTime(int scriptIndex)
{
	RDRN_Module::ManagedGlobals::g_scriptDomain->ScriptResetWaitTime(scriptIndex);
}

static void ManagedScriptTick(int scriptIndex)
{
	RDRN_Module::ManagedGlobals::g_scriptDomain->ScriptTick(scriptIndex);
}

static void ManagedUnload()
{
	RDRN_Module::ManagedGlobals::g_scriptDomain->Unload();
}

#pragma unmanaged
#include <main.h>
#include <Windows.h>
#include <cstdarg>
#include <iostream>

struct ScriptFiberInfo
{
	int m_index;
	void* m_fiberMain;
	void* m_fiberScript;

	bool m_initialized;
	bool m_defect;
};

static HMODULE _instance;

static std::vector<ScriptFiberInfo*> _scriptFibers;

static void ScriptKeyboardMessage(DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow);

static void ScriptMainFiber(LPVOID pv)
{
	ScriptFiberInfo* fi = (ScriptFiberInfo*)pv;

	while (!unload) {
		if (fi->m_defect) {
			SwitchToFiber(fi->m_fiberMain);
			continue;
		}

		if (!fi->m_initialized) {
			if (ManagedScriptInit(fi->m_index, fi->m_fiberMain, fi->m_fiberScript)) {
				fi->m_initialized = true;
			} else {
				fi->m_defect = true;
			}
			SwitchToFiber(fi->m_fiberMain);
			continue;
		}

		ManagedScriptTick(fi->m_index);
		SwitchToFiber(fi->m_fiberMain);
	}
}

static void ScriptMain(int index)
{
	ScriptFiberInfo fi;
	fi.m_index = index;
	fi.m_fiberMain = GetCurrentFiber();
	fi.m_fiberScript = CreateFiber(0, ScriptMainFiber, (LPVOID)&fi);
	fi.m_initialized = false;
	fi.m_defect = false;
	_scriptFibers.push_back(&fi);

	UnmanagedLogWrite("ScriptMain(%d) -> Initialized: %d, defect: %d (main: %p, script: %p)\n", index, (int)fi.m_initialized, (int)fi.m_defect, fi.m_fiberMain, fi.m_fiberScript);

	while (!ManagedScriptExists(fi.m_index)) {
		scriptWait(0);
	}

	UnmanagedLogWrite("ScriptMain(%d) became active!\n", index);

	while (true) {
		int ms = ManagedScriptGetWaitTime(fi.m_index);
		scriptWait(ms);
		ManagedScriptResetWaitTime(fi.m_index);
		SwitchToFiber(fi.m_fiberScript);
	}
}

// uh
// yeah
// it would be nice if shv gave us some more leeway here
#pragma optimize("", off)
static void ScriptMain_Wrapper0() { ScriptMain(0); }
static void ScriptMain_Wrapper1() { ScriptMain(1); }
static void ScriptMain_Wrapper2() { ScriptMain(2); }
static void ScriptMain_Wrapper3() { ScriptMain(3); }
static void ScriptMain_Wrapper4() { ScriptMain(4); }
static void ScriptMain_Wrapper5() { ScriptMain(5); }
static void ScriptMain_Wrapper6() { ScriptMain(6); }
static void ScriptMain_Wrapper7() { ScriptMain(7); }
static void ScriptMain_Wrapper8() { ScriptMain(8); }
static void ScriptMain_Wrapper9() { ScriptMain(9); }
static void ScriptMain_Wrapper10() { ScriptMain(10); }
static void ScriptMain_Wrapper11() { ScriptMain(11); }
static void ScriptMain_Wrapper12() { ScriptMain(12); }
static void ScriptMain_Wrapper13() { ScriptMain(13); }
static void ScriptMain_Wrapper14() { ScriptMain(14); }
static void ScriptMain_Wrapper15() { ScriptMain(15); }
static void ScriptMain_Wrapper16() { ScriptMain(16); }
static void ScriptMain_Wrapper17() { ScriptMain(17); }
static void ScriptMain_Wrapper18() { ScriptMain(18); }
static void ScriptMain_Wrapper19() { ScriptMain(19); }
static void ScriptMain_Wrapper20() { ScriptMain(20); }
#pragma optimize("", on)

static void(*_scriptWrappers[])() = {
	&ScriptMain_Wrapper0,
	&ScriptMain_Wrapper1,
	&ScriptMain_Wrapper2,
	&ScriptMain_Wrapper3,
	&ScriptMain_Wrapper4,
	&ScriptMain_Wrapper5,
	&ScriptMain_Wrapper6,
	&ScriptMain_Wrapper7,
	&ScriptMain_Wrapper8,
	&ScriptMain_Wrapper9,
	&ScriptMain_Wrapper10,
	&ScriptMain_Wrapper11,
	&ScriptMain_Wrapper12,
	&ScriptMain_Wrapper13,
	&ScriptMain_Wrapper14,
	&ScriptMain_Wrapper15,
	&ScriptMain_Wrapper16,
	&ScriptMain_Wrapper17,
	&ScriptMain_Wrapper18,
	&ScriptMain_Wrapper19,
	&ScriptMain_Wrapper20,
};

void RegisterScriptMain(int index)
{
	if (index > 20) {
		//TODO: Log some error?
		return;
	}
	UnmanagedLogWrite("RegisterScriptMain(%d) : %p\n", index, _scriptWrappers[index]);
	scriptRegister(_instance, _scriptWrappers[index]);
}

DWORD WINAPI CleanupThread(LPVOID lparam)
{
	/*
	unload = true;
	FreeConsole();
	//FreeLibraryAndExitThread(static_cast<HMODULE>(_instance), 0);

	if (GetEnvironmentVariableA("SHVDNPro", nullptr, 0) == 0) {
		UnmanagedLogWrite("DllMain detach detected SHVDNPro not running");
		return TRUE;
	}

	SetEnvironmentVariableA("SHVDNPro", nullptr);

	UnmanagedLogWrite("DllMain DLL_PROCESS_DETACH\n");
	//keyboardHandlerUnregister(&ScriptKeyboardMessage);
	
	ManagedUnload();
	scriptUnregister(_instance);
	
	for (auto fi : _scriptFibers) {
		if (fi->m_initialized)
			DeleteFiber(fi->m_fiberScript);
	}*/
	return true;
}

static void ScriptKeyboardMessage(DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow)
{
	bool ctrl = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;

	if (key == 82 && !unload) // R
	{
		
		//ManagedUnload();
		//CreateThread(nullptr, THREAD_ALL_ACCESS, CleanupThread, nullptr, NULL, nullptr);
	}
	
	ManagedScriptKeyboardMessage(key, repeats, scanCode, isExtended, isWithAlt, wasDownBefore, isUpNow);
}
#pragma managed

static void ManagedInitialize()
{
	RDRN_Module::LogManager::WriteLog("Red Dead Redemption II: Network starting ...");

	auto eventSink = gcnew ManagedEventSink();
	System::AppDomain::CurrentDomain->UnhandledException += gcnew System::UnhandledExceptionEventHandler(eventSink, &ManagedEventSink::OnUnhandledException);
}

static void* _fiberControl;

static void ManagedSHVDNProControl()
{
	void* fiber = CreateFiber(0, [](void*) {
		RDRN_Module::LogManager::WriteLog("Control thread initializing");

		LoadScriptDomain();

		SwitchToFiber(_fiberControl);
	}, nullptr);

	SwitchToFiber(fiber);
}

#pragma unmanaged

static void SHVDNProControl()
{
	_fiberControl = GetCurrentFiber();

	scriptWait(0);
	ManagedSHVDNProControl();
}

BOOL APIENTRY DllMain(HMODULE hInstance, DWORD reason, LPVOID lpReserved)
{
	if (reason == DLL_PROCESS_ATTACH) {

		if (GetEnvironmentVariableA("SHVDNPro", nullptr, 0) > 0) {
			UnmanagedLogWrite("DllMain attach detected SHVDNPro already running");
			return TRUE;
		}

		if (AllocConsole())
		{
			freopen("CONIN$", "r", stdin);
			freopen("CONOUT$", "w", stdout);
			freopen("CONOUT$", "w", stderr);
			SetConsoleTitleA("Red Dead Redemption II: Network");
		}

		SetEnvironmentVariableA("SHVDNPro", "Melissa");

		//UnmanagedLogWrite("DllMain DLL_PROCESS_ATTACH\n");

		DisableThreadLibraryCalls(hInstance);
		_instance = hInstance;

		ManagedInitialize();
		scriptRegister(hInstance, SHVDNProControl);
		for (int i = 0; i < 21; i++) {
			RegisterScriptMain(i);
		}
		keyboardHandlerRegister(&ScriptKeyboardMessage);

	} else if (reason == DLL_PROCESS_DETACH) {
		if (GetEnvironmentVariableA("SHVDNPro", nullptr, 0) == 0) {
			UnmanagedLogWrite("DllMain detach detected SHVDNPro not running");
			return TRUE;
		}

		SetEnvironmentVariableA("SHVDNPro", nullptr);

		UnmanagedLogWrite("DllMain DLL_PROCESS_DETACH\n");

		keyboardHandlerUnregister(&ScriptKeyboardMessage);
		scriptUnregister(hInstance);

		for (auto fi : _scriptFibers) {
			DeleteFiber(fi->m_fiberScript);
		}
	}

	return TRUE;
}
