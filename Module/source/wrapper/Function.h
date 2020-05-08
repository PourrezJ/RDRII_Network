#pragma once
#include "NativeHashes.hpp"

namespace RDRN_Module
{
	namespace Native
	{
		public ref class Func abstract sealed
		{
		public:
			static System::UInt64* InvokeManaged(Hash hash, ...array<System::UInt64>^ arguments);
            //generic<typename T> static T Call(Hash hash, ... array<InputArgument^> ^ arguments);			
			//static void Call(Hash hash, ... array<InputArgument^> ^ arguments);
			static System::IntPtr AddStringPool(System::String^ string);

		internal:
			static System::Collections::Generic::List<System::IntPtr>^ UnmanagedStrings = gcnew System::Collections::Generic::List<System::IntPtr>();
			static void ClearStringPool();
		};
	}
}
