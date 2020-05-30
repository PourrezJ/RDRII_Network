#include <cstdio>

#include <Script.h>

#include <ManagedGlobals.h>
#include <Log.h>

#include <Config.h>

#pragma unmanaged
#include <Windows.h>
#undef Yield

#include <UnmanagedLog.h>

static void ScriptSwitchToMainFiber(void* fiber)
{
	SwitchToFiber(fiber);
}
#pragma managed

RDRN_Module::Script::Script()
{
	m_fiberMain = nullptr;
	m_fiberWait = 0;
}

void RDRN_Module::Script::Wait(int ms)
{
#ifdef THROW_ON_WRONG_FIBER_YIELD
	if (GetCurrentFiber() != m_fiberCurrent) {
		throw gcnew System::Exception(System::String::Format("Illegal fiber wait {0} from invalid thread:\n{1}", ms, System::Environment::StackTrace));
	}
#endif

	m_fiberWait = ms;
	ScriptSwitchToMainFiber(m_fiberMain);
}

void RDRN_Module::Script::Yield()
{
	Wait(0);
}

void RDRN_Module::Script::OnInit()
{
}

void RDRN_Module::Script::OnTick()
{
}

void RDRN_Module::Script::OnKeyDown(System::Windows::Forms::KeyEventArgs^ args)
{
}

void RDRN_Module::Script::OnKeyUp(System::Windows::Forms::KeyEventArgs^ args)
{
}

RDRN_Module::Script^ RDRN_Module::Script::GetExecuting()
{
	return RDRN_Module::ManagedGlobals::g_scriptDomain->GetExecuting();
}

void RDRN_Module::Script::WaitExecuting(int ms)
{
	auto script = GetExecuting();
	if (script == nullptr) {
		throw gcnew System::Exception("Illegal call to WaitExecuting() from a non-script fiber!");
	}
	script->Wait(ms);
}

void RDRN_Module::Script::YieldExecuting()
{
	WaitExecuting(0);
}

void RDRN_Module::Script::ProcessOneTick()
{
	System::Tuple<bool, System::Windows::Forms::Keys>^ ev = nullptr;

	while (m_keyboardEvents->TryDequeue(ev)) {
		try {
			if (ev->Item1) {
				OnKeyDown(gcnew System::Windows::Forms::KeyEventArgs(ev->Item2));
			} else {
				OnKeyUp(gcnew System::Windows::Forms::KeyEventArgs(ev->Item2));
			}
		} catch (System::Exception^ ex) {
			if (ev->Item1) {
				RDRN_Module::LogManager::WriteLog("*** Exception during OnKeyDown: {0}", ex->ToString());
			} else {
				RDRN_Module::LogManager::WriteLog("*** Exception during OnKeyUp: {0}", ex->ToString());
			}
		}
	}

	try {
		OnTick();
	} catch (System::Exception^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Exception during OnTick: {0}", ex->ToString());
	} catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Unmanaged exception during OnTick in {0}", GetType()->FullName);
	}
}
