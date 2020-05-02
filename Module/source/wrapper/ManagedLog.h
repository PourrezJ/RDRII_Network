#pragma once

namespace RDRN_Module
{
	public ref class LogManager {
	public:

		enum class LogLevel {
			Trace,
			Debug,
			Information,
			Warning,
			Error,
			Critical
		};

		static LogLevel MinLevel = LogLevel::Information;

		static void WriteLog(System::String^ format, ... array<System::Object^>^ args);

		static void WriteLog(LogLevel level, System::String^ format, ...array<System::Object^>^ args);

		static void Exception(System::Exception^ exception, ...array<System::Object^>^ args);
	};
}
