#include "ScriptDomain.h"

#include "Function.h"

#include "ManagedGlobals.h"
#include "ManagedLog.h"

#pragma unmanaged
#include <Windows.h>
#pragma managed

RDRN_Module::ScriptDomain::ScriptDomain()
{
	System::AppDomain::CurrentDomain->UnhandledException += gcnew System::UnhandledExceptionEventHandler(this, &RDRN_Module::ScriptDomain::OnUnhandledException);
	System::AppDomain::CurrentDomain->AssemblyResolve += gcnew System::ResolveEventHandler(this, &RDRN_Module::ScriptDomain::OnAssemblyResolve);

	RDRN_Module::ManagedGlobals::g_scriptDomain = this;
}

void RDRN_Module::ScriptDomain::FindAllTypes()
{
	auto ret = gcnew System::Collections::Generic::List<System::Type^>();

	CurrentDir = System::Reflection::Assembly::GetExecutingAssembly()->Location;
	CurrentDir = System::IO::Path::GetDirectoryName(CurrentDir);

	auto file = System::IO::Path::Combine(CurrentDir, "Scripts\\RDRN_Core.dll");

	auto fileName = System::IO::Path::GetFileName(file);

	System::Reflection::Assembly^ assembly = nullptr;
	try {
		//System::Reflection::Assembly::LoadFrom(cefcore);
		assembly = System::Reflection::Assembly::LoadFrom(file);
	}
	catch (System::Exception ^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Assembly load exception for {0}: {1}", fileName, ex->ToString());
	}
	catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Unmanaged exception while loading assembly!");
	}

	RDRN_Module::LogManager::WriteLog("Loaded assembly {0}", assembly);

	try {
		for each (auto type in assembly->GetTypes()) {
			if (!type->IsSubclassOf(RDRN_Module::Script::typeid)) {
				continue;
			}

			ret->Add(type);

			RDRN_Module::LogManager::WriteLog("Registered type {0}", type->FullName);
		}
	}
	catch (System::Reflection::ReflectionTypeLoadException ^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Exception while iterating types: {0}", ex->ToString());
		for each (auto loaderEx in ex->LoaderExceptions) {
			RDRN_Module::LogManager::WriteLog("***    {0}", loaderEx->ToString());
		}
		//continue;
	}
	catch (System::Exception ^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Exception while iterating types: {0}", ex->ToString());
		//continue;
	}
	catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Unmanaged exception while iterating types");
		//continue;
	}

	m_types = ret->ToArray();
	m_scripts = gcnew array<RDRN_Module::Script^>(m_types->Length);

	RDRN_Module::LogManager::WriteLog("{0} script types found:", m_types->Length);
	for (int i = 0; i < m_types->Length; i++) {
		RDRN_Module::LogManager::WriteLog("  {0}: {1}", i, m_types[i]->FullName);

		RDRN_Module::Script ^ script = nullptr;
		try
		{
            script = static_cast<RDRN_Module::Script ^>( System::Activator::CreateInstance(m_types[i]));
		}
		catch (System::Reflection::ReflectionTypeLoadException ^ ex)
		{
			RDRN_Module::LogManager::WriteLog("*** Exception while instantiating script: {0}", ex->ToString());
			for each(auto loaderEx in ex->LoaderExceptions)
			{
				RDRN_Module::LogManager::WriteLog("***    {0}", loaderEx->ToString());
			}
			continue;
		}
		catch (System::Exception ^ ex)
		{
			RDRN_Module::LogManager::WriteLog("*** Exception while instantiating script: {0}", ex->ToString());
            continue;
		}
		catch (...)
		{
			RDRN_Module::LogManager::WriteLog("*** Unmanaged exception while instantiating script!");
            continue;
		}

		try
        {
            script->OnInit();
            m_scripts[i] = script;
        }
        catch (System::Exception ^ ex)
        {
            RDRN_Module::LogManager::WriteLog("*** Exception in script OnInit: {0}", ex->ToString());
            continue;
        }
        catch (...)
        {
			RDRN_Module::LogManager::WriteLog("*** Unmanaged exception in script OnInit!");
            continue;
        }
	}
}


RDRN_Module::Script^ RDRN_Module::ScriptDomain::GetExecuting()
{
	void* currentFiber = GetCurrentFiber();

	// I don't know if GetCurrentFiber ever returns null, but whatever
	if (currentFiber == nullptr) {
		return nullptr;
	}

	for each (auto script in m_scripts) {
		if (script != nullptr && script->m_fiberCurrent == currentFiber) {
			return script;
		}
	}

	return nullptr;
}

void RDRN_Module::ScriptDomain::TickAllScripts() 
{
	if (m_scripts == nullptr)
		return;

    for (int i = 0; i < m_scripts->Length; i++)
	{
        RDRN_Module::ScriptDomain::ScriptTick(i);
	}
}

void RDRN_Module::ScriptDomain::ScriptTick(int scriptIndex)
{
	try {
		auto script = m_scripts[scriptIndex];
		if (script != nullptr) {
			script->ProcessOneTick();
		}
	} catch (System::Exception^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Exception in script ProcessOneTick: {0}", ex->ToString());
	} catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Unmanaged exception in script ProcessOneTick!");
	}

	try {
		RDRN_Module::Native::Function::ClearStringPool();
	} catch (System::Exception^ ex) {
		RDRN_Module::LogManager::WriteLog("*** Exception while clearing string pool: {0}", ex->ToString());
	} catch (...) {
		RDRN_Module::LogManager::WriteLog("*** Unmanaged exception while clearning string pool!");
	}
}


void RDRN_Module::ScriptDomain::QueueKeyboardEvent(System::Tuple<bool, System::Windows::Forms::Keys>^ ev)
{
	for each (auto script in m_scripts) {
		if (script == nullptr) {
			continue;
		}
		script->m_keyboardEvents->Enqueue(ev);
	}
}

void RDRN_Module::ScriptDomain::OnUnhandledException(System::Object^ sender, System::UnhandledExceptionEventArgs^ e)
{
	RDRN_Module::LogManager::WriteLog("*** Unhandled exception: {0}", e->ExceptionObject->ToString());
}

System::Reflection::Assembly^ RDRN_Module::ScriptDomain::OnAssemblyResolve(System::Object^ sender, System::ResolveEventArgs^ args)
{
	if (args->RequestingAssembly != nullptr) {
		RDRN_Module::LogManager::WriteLog("Resolving assembly: \"{0}\" from: \"{1}\"", args->Name, args->RequestingAssembly->FullName);
	} else {
		RDRN_Module::LogManager::WriteLog("Resolving assembly: \"{0}\"", args->Name);
	}

	auto exeAssembly = System::Reflection::Assembly::GetExecutingAssembly();
	if (args->Name == exeAssembly->FullName) {
		RDRN_Module::LogManager::WriteLog("  Returning exeAssembly: \"{0}\"", exeAssembly->FullName);
		return exeAssembly;
	}

	return args->RequestingAssembly;

	//RDR2::WriteLog("  Returning nullptr");
	//return nullptr;
}
