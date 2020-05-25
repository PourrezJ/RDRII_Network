
#pragma once
#undef Yield

namespace RDRN_Module
{
#pragma region Forward Declarations
	ref class ScriptDomain;
#pragma endregion

	[System::AttributeUsage(System::AttributeTargets::Class, AllowMultiple = true)]
	public ref class RequireScript : System::Attribute
	{
	public:
		RequireScript(System::Type^ dependency) : _dependency(dependency) { }

	internal:
		System::Type^ _dependency;
	};

	public ref class Script abstract
	{
	public:
		Script();

		static void Wait(int ms);
		static void Yield();

		event System::EventHandler^ Tick;
		event System::Windows::Forms::KeyEventHandler^ KeyUp;
		event System::Windows::Forms::KeyEventHandler^ KeyDown;
		event System::EventHandler^ Aborted;

		property System::String^ Name
		{
			System::String^ get()
			{
				return GetType()->FullName;
			}
		}
		property System::String^ Filename
		{
			System::String^ get()
			{
				return _filename;
			}
		}

		void Abort();

		virtual System::String^ ToString() override
		{
			return Name;
		}

	protected:
		property int Interval
		{
			int get();
			void set(int value);
		}

	internal:
		void MainLoop();

		int _interval = 0;
		bool _running = false;
		System::String^ _filename;
		ScriptDomain^ _scriptdomain;
		System::Threading::Thread^ _thread;
		System::Threading::AutoResetEvent^ _waitEvent = gcnew System::Threading::AutoResetEvent(false);
		System::Threading::AutoResetEvent^ _continueEvent = gcnew System::Threading::AutoResetEvent(false);
		System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::KeyEventArgs^>^>^ _keyboardEvents = gcnew System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::KeyEventArgs^>^>();
	};
}