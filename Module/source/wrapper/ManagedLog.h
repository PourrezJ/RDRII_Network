#pragma once

namespace RDRN_Module
{
	public ref class LogManager {
	public:
		static void WriteLog(System::String^ format, ... array<System::Object^>^ args);
	};
}
