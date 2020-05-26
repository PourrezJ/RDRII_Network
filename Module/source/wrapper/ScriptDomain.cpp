
#include "ScriptDomain.h"

using namespace System;
using namespace System::Threading;
using namespace System::Reflection;
using namespace System::Collections::Generic;
namespace WinForms = System::Windows::Forms;

namespace
{
	inline void SignalAndWait(AutoResetEvent^ toSignal, AutoResetEvent^ toWaitOn)
	{
		toSignal->Set();
		toWaitOn->WaitOne();
	}
	inline bool SignalAndWait(AutoResetEvent^ toSignal, AutoResetEvent^ toWaitOn, int timeout)
	{
		toSignal->Set();
		return toWaitOn->WaitOne(timeout);
	}
}

namespace RDRN_Module
{	
	Assembly^ HandleResolve(Object^ sender, ResolveEventArgs^ args)
	{
		auto assembly = Script::typeid->Assembly;
		auto assemblyName = gcnew AssemblyName(args->Name);

		if (assemblyName->Name->StartsWith("ScriptHookVDotNet", StringComparison::CurrentCultureIgnoreCase))
		{
			if (assemblyName->Version->Major != assembly->GetName()->Version->Major)
			{
				RDRN_Module::LogManager::WriteLog("[WARNING] A script references v" + assemblyName->Version->ToString(3) + " which may not be compatible with the current v" + assembly->GetName()->Version->ToString(3), ".");
			}

			return assembly;
		}

		return nullptr;
	}
	void HandleUnhandledException(Object^ sender, UnhandledExceptionEventArgs^ args)
	{
		if (!args->IsTerminating)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Caught unhandled exception: \n" + args->ExceptionObject->ToString());
		}
		else
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Caught fatal unhandled exception: \n" + args->ExceptionObject->ToString());
		}
	}

	ScriptDomain::ScriptDomain() : _appdomain(System::AppDomain::CurrentDomain), _executingThreadId(Thread::CurrentThread->ManagedThreadId)
	{
		sCurrentDomain = this;

		_appdomain->AssemblyResolve += gcnew ResolveEventHandler(&HandleResolve);
		_appdomain->UnhandledException += gcnew UnhandledExceptionEventHandler(&HandleUnhandledException);

		RDRN_Module::LogManager::WriteLog("[INFO] Created new script domain with  " + ScriptDomain::typeid->Assembly->GetName()->Version->ToString(3));
	}
	ScriptDomain::~ScriptDomain()
	{
		CleanupStrings();
	}

	ScriptDomain^ ScriptDomain::Load(String^ path)
	{
		path = IO::Path::GetFullPath(path);
		
		auto setup = gcnew AppDomainSetup();
		setup->ApplicationBase = path;
		setup->ShadowCopyFiles = "true";
		setup->ShadowCopyDirectories = path;

		auto appdomain = System::AppDomain::CreateDomain("ScriptDomain_" + (path->GetHashCode() * Environment::TickCount).ToString("X"), nullptr, setup, gcnew Security::PermissionSet(Security::Permissions::PermissionState::Unrestricted));
		appdomain->InitializeLifetimeService();

		ScriptDomain^ scriptdomain = nullptr;

		try
		{
			scriptdomain = static_cast<ScriptDomain^>(appdomain->CreateInstanceFromAndUnwrap(ScriptDomain::typeid->Assembly->Location, ScriptDomain::typeid->FullName));
		}
		catch (Exception^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to create script domain '" + appdomain->FriendlyName + "': \n " + ex->ToString());

			System::AppDomain::Unload(appdomain);

			return nullptr;
		}

		RDRN_Module::LogManager::WriteLog("[INFO] Loading scripts from '" + path + "' into script domain '" + appdomain->FriendlyName + "' ...");

		if (IO::Directory::Exists(path))
		{
			auto filenameAssemblies = gcnew List<String^>();

			try
			{
				filenameAssemblies->AddRange(IO::Directory::GetFiles(path, "*.dll", IO::SearchOption::AllDirectories));
			}
			catch (Exception^ ex)
			{
				RDRN_Module::LogManager::WriteLog("[ERROR] Failed to reload scripts: \n" + ex->ToString());

				System::AppDomain::Unload(appdomain);

				return nullptr;
			}

			for each (String ^ filename in filenameAssemblies)
			{
				scriptdomain->LoadAssembly(filename);
			}
		}
		else
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to reload scripts because the directory is missing.");
		}

		return scriptdomain;
	}

	bool ScriptDomain::LoadAssembly(String^ filename)
	{
		Assembly^ assembly = nullptr;

		try
		{
			assembly = Assembly::LoadFrom(filename);
		}
		catch (Exception^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to load assembly '" + IO::Path::GetFileName(filename) + "': \n" + ex->ToString());

			return false;
		}

		return LoadAssembly(filename, assembly);
	}

	bool ScriptDomain::LoadAssembly(String^ filename, Assembly^ assembly)
	{
		unsigned int count = 0;

		try
		{
			for each (auto type in assembly->GetTypes())
			{
				if (!type->IsSubclassOf(Script::typeid))
				{
					continue;
				}

				count++;
				_scriptTypes->Add(gcnew Tuple<String^, Type^>(filename, type));
			}
		}
		catch (ReflectionTypeLoadException^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to load assembly '" + IO::Path::GetFileName(filename) + "': \n" + ex->ToString());

			return false;
		}

		RDRN_Module::LogManager::WriteLog("[INFO] Found " + count.ToString() + " script(s) in '" + IO::Path::GetFileName(filename) + "'.");

		return count != 0;
	}

	void ScriptDomain::Unload(ScriptDomain^% domain)
	{
		RDRN_Module::LogManager::WriteLog("[INFO] Unloading script domain ...");

		domain->Abort();

		System::AppDomain^ appdomain = domain->AppDomain;

		delete domain;

		try
		{
			System::AppDomain::Unload(appdomain);
		}
		catch (Exception^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to unload deleted script domain: \n" + ex->ToString());
		}

		domain = nullptr;

		GC::Collect();
	}

	Script^ ScriptDomain::InstantiateScript(Type^ scriptType)
	{
		if (!scriptType->IsSubclassOf(Script::typeid))
		{
			return nullptr;
		}

		RDRN_Module::LogManager::WriteLog("[INFO] Instantiating script '" + scriptType->FullName + "' in script domain '" + Name + "' ...");

		try
		{
			return static_cast<Script^>(Activator::CreateInstance(scriptType));
		}
		catch (MissingMethodException^)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to instantiate script '" + scriptType->FullName + "' because no public default constructor was found.");
		}
		catch (TargetInvocationException^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to instantiate script '" + scriptType->FullName + "' because constructor threw an exception: \n" + ex->InnerException->ToString());
		}
		catch (Exception^ ex)
		{
			RDRN_Module::LogManager::WriteLog("[ERROR] Failed to instantiate script '" + scriptType->FullName + "': \n" + ex->ToString());
		}

		return nullptr;
	}

	void ScriptDomain::Start()
	{
		if (_runningScripts->Count != 0 || _scriptTypes->Count == 0)
		{
			return;
		}

		String^ assemblyPath = Assembly::GetExecutingAssembly()->Location;
		String^ assemblyFilename = IO::Path::GetFileNameWithoutExtension(assemblyPath);

		RDRN_Module::LogManager::WriteLog("[INFO] Starting " + _scriptTypes->Count.ToString() + " script(s) ...");

		for each (auto scriptType in _scriptTypes)
		{
			Script^ script = InstantiateScript(scriptType->Item2);

			if (Object::ReferenceEquals(script, nullptr))
			{
				continue;
			}

			script->_running = true;
			script->_filename = scriptType->Item1;
			script->_scriptdomain = this;
			script->_thread = gcnew Thread(gcnew ThreadStart(script, &Script::MainLoop));

			script->_thread->Start();

			RDRN_Module::LogManager::WriteLog("[INFO] Started script '" + script->Name + "'.");

			_runningScripts->Add(script);
		}
	}
	void ScriptDomain::Abort()
	{
		RDRN_Module::LogManager::WriteLog("[INFO] Stopping " + _runningScripts->Count.ToString() + " script(s) ...");

		for each (Script ^ script in _runningScripts)
		{
			script->Abort();

			delete script;
		}

		_scriptTypes->Clear();
		_runningScripts->Clear();

		GC::Collect();
	}
	void ScriptDomain::AbortScript(Script^ script)
	{
		if (Object::ReferenceEquals(script->_thread, nullptr))
		{
			return;
		}

		script->_running = false;

		script->_thread->Abort();
		script->_thread = nullptr;

		RDRN_Module::LogManager::WriteLog("[INFO] Aborted script '" + script->Name + "'.");
	}
	void ScriptDomain::DoTick()
	{
		// Execute scripts
		for each (Script ^ script in _runningScripts)
		{
			if (!script->_running)
			{
				continue;
			}

			_executingScript = script;

			while ((script->_running = SignalAndWait(script->_continueEvent, script->_waitEvent, 5000)) && _taskQueue->Count > 0)
			{
				_taskQueue->Dequeue()->Run();
			}

			_executingScript = nullptr;

			if (!script->_running)
			{
				RDRN_Module::LogManager::WriteLog("[ERROR] Script '" + script->Name + "' is not responding! Aborting ...");

				AbortScript(script);
				continue;
			}
		}

		// Clean up pinned strings
		CleanupStrings();
	}
	void ScriptDomain::DoKeyboardMessage(WinForms::Keys key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
	{
		const int keycode = static_cast<int>(key);

		if (keycode < 0 || keycode >= _keyboardState->Length)
		{
			return;
		}

		_keyboardState[keycode] = status;

		if (_recordKeyboardEvents)
		{
			if (statusCtrl)
			{
				key = key | WinForms::Keys::Control;
			}
			if (statusShift)
			{
				key = key | WinForms::Keys::Shift;
			}
			if (statusAlt)
			{
				key = key | WinForms::Keys::Alt;
			}

			auto args = gcnew WinForms::KeyEventArgs(key);
			auto eventinfo = gcnew Tuple<bool, WinForms::KeyEventArgs^>(status, args);

			for each (Script ^ script in _runningScripts)
			{
				script->_keyboardEvents->Enqueue(eventinfo);
			}
		}
	}

	void ScriptDomain::PauseKeyboardEvents(bool pause)
	{
		_recordKeyboardEvents = !pause;
	}
	void ScriptDomain::ExecuteTask(IScriptTask^ task)
	{
		if (Thread::CurrentThread->ManagedThreadId == _executingThreadId)
		{
			task->Run();
		}
		else
		{
			_taskQueue->Enqueue(task);

			SignalAndWait(ExecutingScript->_waitEvent, ExecutingScript->_continueEvent);
		}
	}
	IntPtr ScriptDomain::PinString(String^ string)
	{
		const int size = Text::Encoding::UTF8->GetByteCount(string);
		IntPtr handle(new unsigned char[size + 1]());

		Runtime::InteropServices::Marshal::Copy(Text::Encoding::UTF8->GetBytes(string), 0, handle, size);

		_pinnedStrings->Add(handle);

		return handle;
	}
	void ScriptDomain::CleanupStrings()
	{
		for each (IntPtr handle in _pinnedStrings)
		{
			delete[] handle.ToPointer();
		}

		_pinnedStrings->Clear();
	}
	String^ ScriptDomain::LookupScriptFilename(Type^ type)
	{
		for each (auto scriptType in _scriptTypes)
		{
			if (scriptType->Item2 == type)
			{
				return scriptType->Item1;
			}
		}

		return String::Empty;
	}
	Object^ ScriptDomain::InitializeLifetimeService()
	{
		return nullptr;
	}
}
