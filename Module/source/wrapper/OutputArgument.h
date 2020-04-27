#pragma once

namespace RDRN_Module
{
	namespace Native
	{
		public ref class OutputArgument
		{
		public:
			OutputArgument();
			OutputArgument(System::Object^ initvalue);
			~OutputArgument();

			generic <typename T> T GetResult();

		internal:
			void* GetPointer();

		protected:
			!OutputArgument();

			void* _storage;
		};
	}
}