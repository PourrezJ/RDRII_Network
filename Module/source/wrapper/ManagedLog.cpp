#include "ManagedLog.h"

System::String^ GetPath()
{
	return System::IO::Path::GetDirectoryName(System::Reflection::Assembly::GetExecutingAssembly()->Location);
}

void RDRN_Module::LogManager:: WriteLog(System::String^ format, ... array<System::Object^>^ args)
{
	try {
		auto writerPath = System::IO::Path::Combine(GetPath(), "..//logs//RDRNetwork.log");
		auto writer = gcnew System::IO::StreamWriter(writerPath, true);
		auto text = ("[{0}] {1}", System::DateTime::Now.ToString("HH:mm:ss.fff"), System::String::Format(format, args));
		writer->WriteLine(text);
		System::Console::WriteLine(text);
		delete writer;
	}
	catch (...) {}
}

void RDRN_Module::LogManager::WriteLog(LogLevel level, System::String^ format, ... array<System::Object^>^ args)
{
	if (LogManager::MinLevel > level)
		return;

	try {
		auto writerPath = System::IO::Path::Combine(GetPath(), "..//logs//RDRNetwork.log");
		auto writer = gcnew System::IO::StreamWriter(writerPath, true);
		auto text = ("[{0}] {1}: {2}", System::DateTime::Now.ToString("HH:mm:ss.fff"), level, System::String::Format(format, args));
		writer->WriteLine(text);
		System::Console::WriteLine(text);
		delete writer;
	}
	catch (...) {}
}

void RDRN_Module::LogManager::Exception(System::Exception^ exception, ... array<System::Object^>^ args)
{
	try {
		auto writerPath = System::IO::Path::Combine(GetPath(), "..//logs//Exception.log");
		auto writer = gcnew System::IO::StreamWriter(writerPath, true);
		auto text = ("[{0}] {1}", System::DateTime::Now.ToString("HH:mm:ss.fff"), System::String::Format(exception->ToString(), args));
		writer->WriteLine(text);
		System::Console::WriteLine(text);
		delete writer;
	}
	catch (...) {}
}
