
#pragma once

#include "Script.h"

namespace RDRN_Module
{
	private interface class IScriptTask
	{
		void Run();
	};

	private ref class ScriptDomain sealed : public System::MarshalByRefObject
	{
	public:
		ScriptDomain();
		~ScriptDomain();

		static property Script^ ExecutingScript
		{
			Script^ get()
			{
				if (System::Object::ReferenceEquals(sCurrentDomain, nullptr))
				{
					return nullptr;
				}

				return sCurrentDomain->_executingScript;
			}
		}
		static property ScriptDomain^ CurrentDomain
		{
			inline ScriptDomain^ get()
			{
				return sCurrentDomain;
			}
		}

		static ScriptDomain^ Load(System::String^ path);
		static void Unload(ScriptDomain^% domain);

		property System::String^ Name
		{
			inline System::String^ get()
			{
				return _appdomain->FriendlyName;
			}
		}
		property System::AppDomain^ AppDomain
		{
			inline System::AppDomain^ get()
			{
				return _appdomain;
			}
		}

		void Start();
		void Abort();
		static void AbortScript(Script^ script);
		void DoTick();
		void DoKeyboardMessage(System::Windows::Forms::Keys key, bool status, bool statusCtrl, bool statusShift, bool statusAlt);

		void PauseKeyboardEvents(bool pause);
		void ExecuteTask(IScriptTask^ task);
		System::IntPtr PinString(System::String^ string);
		inline bool IsKeyPressed(System::Windows::Forms::Keys key)
		{
			return _keyboardState[static_cast<int>(key)];
		}
		inline System::String^ LookupScriptFilename(Script^ script)
		{
			return LookupScriptFilename(script->GetType());
		}
		System::String^ LookupScriptFilename(System::Type^ scripttype);
		System::Object^ InitializeLifetimeService() override;

	private:
		bool LoadAssembly(System::String^ filename);
		bool LoadAssembly(System::String^ filename, System::Reflection::Assembly^ assembly);
		Script^ InstantiateScript(System::Type^ scripttype);
		void CleanupStrings();

		static ScriptDomain^ sCurrentDomain;
		System::AppDomain^ _appdomain;
		int _executingThreadId;
		Script^ _executingScript;
		System::Collections::Generic::List<Script^>^ _runningScripts = gcnew System::Collections::Generic::List<Script^>();
		System::Collections::Generic::Queue<IScriptTask^>^ _taskQueue = gcnew System::Collections::Generic::Queue<IScriptTask^>();
		System::Collections::Generic::List<System::IntPtr>^ _pinnedStrings = gcnew System::Collections::Generic::List<System::IntPtr>();
		System::Collections::Generic::List<System::Tuple<System::String^, System::Type^>^>^ _scriptTypes = gcnew System::Collections::Generic::List<System::Tuple<System::String^, System::Type^>^>();
		bool _recordKeyboardEvents = true;
		array<bool>^ _keyboardState = gcnew array<bool>(256);
	};
}
