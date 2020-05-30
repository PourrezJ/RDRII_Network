#pragma once

namespace RDRN_Module
{
	public ref class Script abstract
	{
	internal:
		void* m_fiberMain;
		void* m_fiberCurrent;
		int m_fiberWait;
		System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::Keys>^>^ m_keyboardEvents = gcnew System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::Keys>^>();

	public:
		Script();

		void Wait(int ms);
		void Yield();

		virtual void OnInit();
		virtual void OnTick();
		virtual void OnKeyDown(System::Windows::Forms::KeyEventArgs^ args);
		virtual void OnKeyUp(System::Windows::Forms::KeyEventArgs^ args);

		static property System::String^ CurrentDir
		{
			System::String^ get()
			{
				auto dir = System::Reflection::Assembly::GetExecutingAssembly()->Location;
				return System::IO::Path::GetDirectoryName(dir);
			}
		}

		static Script^ GetExecuting();
		static void WaitExecuting(int ms);
		static void YieldExecuting();

	internal:
		void ProcessOneTick();
	};
}
