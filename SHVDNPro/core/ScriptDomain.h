#pragma once

#include <Script.h>

namespace RDRN_Module
{
	public ref class ScriptDomain : public System::MarshalByRefObject
	{
	internal:
		array<System::Type^>^ m_types;
		array<RDRN_Module::Script^>^ m_scripts;

	public:
		ScriptDomain();

	public:
		void FindAllTypes();

		RDRN_Module::Script^ GetExecuting();

		bool ScriptInit(int scriptIndex, void* fiberMain, void* fiberScript);
		bool ScriptExists(int scriptIndex);
		int ScriptGetWaitTime(int scriptIndex);
		void ScriptResetWaitTime(int scriptIndex);
		void ScriptTick(int scriptIndex);

		void QueueKeyboardEvent(System::Tuple<bool, System::Windows::Forms::Keys>^ ev);

		void OnUnhandledException(System::Object^ sender, System::UnhandledExceptionEventArgs^ e);
		System::Reflection::Assembly^ OnAssemblyResolve(System::Object^ sender, System::ResolveEventArgs^ args);
	};
}
