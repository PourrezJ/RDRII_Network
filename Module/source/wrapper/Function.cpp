#include "../invoker/invoker.hpp"
#include "Function.h"
#include "ManagedGlobals.h"
#include "ManagedLog.h"


#pragma managed

System::UInt64* RDRN_Module::Native::Func::InvokeManaged(Hash hash, ... array<System::UInt64> ^arguments)
{
	try
	{
		rh2::Invoker::NativeInit((rh2::NativeHash)hash);

		for each (System::UInt64 data in arguments) {
			rh2::Invoker::NativePush(data);
		}

		return rh2::Invoker::NativeCall();
	}
	catch (...)
	{
		RDRN_Module::LogManager::WriteLog("Invoke Managed Error");
	}
	return nullptr;
}

System::IntPtr RDRN_Module::Native::Func::AddStringPool(System::String^ string)
{
	auto managedBuffer = System::Text::Encoding::UTF8->GetBytes(string);
	unsigned char* buffer = new unsigned char[managedBuffer->Length + 1];
	buffer[managedBuffer->Length] = '\0';
	System::IntPtr ret(buffer);
	System::Runtime::InteropServices::Marshal::Copy(managedBuffer, 0, ret, managedBuffer->Length);
	RDRN_Module::Native::Func::UnmanagedStrings->Add(ret);
	return ret;
}

void RDRN_Module::Native::Func::ClearStringPool()
{
	for each (auto ptr in RDRN_Module::Native::Func::UnmanagedStrings) {
		delete[] ptr.ToPointer();
	}
	RDRN_Module::Native::Func::UnmanagedStrings->Clear();
}
