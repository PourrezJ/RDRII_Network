#pragma once

namespace RDRN_Module
{
	namespace Native
	{
		public interface class INativeValue
		{
			property System::UInt64 NativeValue
			{
				System::UInt64 get();
				void set(System::UInt64 value);
			};
		};
	}
}
