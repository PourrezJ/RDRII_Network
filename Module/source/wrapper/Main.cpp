#pragma once

#include "Main.hpp"
#include "ManagedLog.h"
#include "ManagedGlobals.h"
#include "Input.h"

#include "../core/core.hpp"
#include "../util/fiber.hpp"
#include "../hooking/input-hook.hpp"
#include "../core/logs.hpp"

#pragma unmanaged
bool sGameReloaded;

namespace rh2
{
    void ClrInit() 
    {
        rh2::logs::g_hLog->log("Clr Init");

        ManagedInit();
        /*
        while (true) {
            ManagedTick();
            rh2::ScriptWait(std::chrono::milliseconds(0));
        }*/
    }

    void ClrTick() 
    {
        ManagedTick();
    }
}



#pragma managed
ref class ManagedEventSink
{
  public:
    void OnUnhandledException(System::Object ^ sender, System::UnhandledExceptionEventArgs ^ args)
    {
        RDRN_Module::LogManager::WriteLog("*** Unhandled exception: {0}", args->ExceptionObject->ToString());
    }
};


void ManagedInit()
{
    RDRN_Module::LogManager::WriteLog("Script Domain Initializing");
    
    auto eventSink = gcnew ManagedEventSink();
    System::AppDomain::CurrentDomain->UnhandledException += gcnew System::UnhandledExceptionEventHandler(eventSink, &ManagedEventSink::OnUnhandledException);
    
    auto curDir = System::IO::Path::GetDirectoryName(
    System::Reflection::Assembly::GetExecutingAssembly()->Location);

    RDRN_Module::LogManager::WriteLog(curDir);

    auto setup                   = gcnew System::AppDomainSetup();
    setup->ApplicationBase       = System::IO::Path::GetFullPath(curDir + "\\Scripts");
    setup->ShadowCopyFiles       = "true"; // !?
    setup->ShadowCopyDirectories = setup->ApplicationBase;

    auto appDomainName =
        "ScriptDomain_" + (curDir->GetHashCode() * System::Environment::TickCount).ToString("X");
    auto appDomainPermissions = gcnew System::Security::PermissionSet(
        System::Security::Permissions::PermissionState::Unrestricted);

    RDRN_Module::ManagedGlobals::g_appDomain =
        System::AppDomain::CreateDomain(appDomainName, nullptr, setup, appDomainPermissions);
    RDRN_Module::ManagedGlobals::g_appDomain->InitializeLifetimeService();

    RDRN_Module::LogManager::WriteLog("Created AppDomain \"{0}\"",
                          RDRN_Module::ManagedGlobals::g_appDomain->FriendlyName);

	auto typeScriptDomain = RDRN_Module::ScriptDomain::typeid;
    try
    {
        RDRN_Module::ManagedGlobals::g_scriptDomain = static_cast<RDRN_Module::ScriptDomain ^>(
            RDRN_Module::ManagedGlobals::g_appDomain->CreateInstanceFromAndUnwrap(
                typeScriptDomain->Assembly->Location, typeScriptDomain->FullName));
    }
    catch (System::Exception ^ ex)
    {
        RDRN_Module::LogManager::WriteLog("*** Failed to create ScriptDomain: {0}", ex->ToString());
        if (ex->InnerException != nullptr)
        {
            RDRN_Module::LogManager::WriteLog("*** InnerException: {0}", ex->InnerException->ToString());
        }
        return;
    }
    catch (...)
    {
        RDRN_Module::LogManager::WriteLog("*** Failed to create ScriptDomain beacuse of unmanaged exception");
        return;
    }

    RDRN_Module::LogManager::WriteLog("Created ScriptDomain!");

    RDRN_Module::ManagedGlobals::g_scriptDomain->FindAllTypes();
}

void ManagedTick() {
    if (RDRN_Module::ManagedGlobals::g_scriptDomain != nullptr) {
        RDRN_Module::ManagedGlobals::g_scriptDomain->TickAllScripts();
    }
}