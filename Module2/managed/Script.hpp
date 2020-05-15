
#pragma managed
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
		RequireScript(System::Type ^dependency) : _dependency(dependency) { }

	internal:
		System::Type ^_dependency;
	};

	/// <summary>
	/// A base class for all user scripts to inherit.
	/// The Hook will only detect and starts Scripts that inherit directly from this class and have a default(parameterless) public constructor.
	/// </summary>
	public ref class Script abstract
	{
	public:
		Script();

		/// <summary>
		/// Pauses execution of the script for a specific amount of time.
		/// Must be called inside the main script loop - The OnTick or any sub methods of it.
		/// </summary>
		/// <param name="ms">The time in ms to pause for</param>
		static void Wait(int ms);
		/// <summary>
		/// Yields the execution of the script for 1 frame.
		/// </summary>
		static void Yield();

		/// <summary>
		/// An event that is raised every tick of the script. 
		/// Put code that needs to be looped each frame in here.
		/// </summary>
		event System::EventHandler ^Tick;
		/// <summary>
		/// An event that is raised when a key is lifted.
		/// The <see cref="System::Windows::Forms::KeyEventArgs"/> contains the key that was lifted.
		/// </summary>
		event System::Windows::Forms::KeyEventHandler ^KeyUp;
		/// <summary>
		/// An event that is raised when a key is first pressed.
		/// The <see cref="System::Windows::Forms::KeyEventArgs"/> contains the key that was pressed.
		/// </summary>
		event System::Windows::Forms::KeyEventHandler ^KeyDown;
		/// <summary>
		/// An event that is raised when this script gets aborted for any reason.
		/// This should be used for cleaning up anything created during this script
		/// </summary>
		event System::EventHandler ^Aborted;

		/// <summary>
		/// Gets the name of this <see cref="Script"/>.
		/// </summary>
		property System::String ^Name
		{
			System::String ^get()
			{
				return GetType()->FullName;
			}
		}
		/// <summary>
		/// Gets the filename of this <see cref="Script"/>.
		/// </summary>
		property System::String ^Filename
		{
			System::String ^get()
			{
				return _filename;
			}
		}

		/// <summary>
		/// Gets the Directory where this <see cref="Script"/> is stored.
		/// </summary>
		property System::String ^BaseDirectory
		{
			System::String ^get()
			{
				return System::IO::Path::GetDirectoryName(_filename);
			}
		}

		/// <summary>
		/// Stops execution of this <see cref="Script"/> indefinitely.
		/// </summary>
		void Abort();

		/// <summary>
		/// Returns a string that represents this <see cref="Script"/>.
		/// </summary>
		virtual System::String ^ToString() override
		{
			return Name;
		}

	protected:
		/// <summary>
		/// Gets or sets the interval in ms between <see cref="Tick"/> for this <see cref="Script"/>.
		/// Default value is 0 meaning the event will execute once each frame.
		/// </summary>
		property int Interval
		{
			int get();
			void set(int value);
		}

	internal:
		void MainLoop();

		int _interval = 0;
		bool _running = false;
		System::String ^_filename;
		ScriptDomain ^_scriptdomain;
		System::Threading::Thread ^_thread;
		System::Threading::AutoResetEvent ^_waitEvent = gcnew System::Threading::AutoResetEvent(false);
		System::Threading::AutoResetEvent ^_continueEvent = gcnew System::Threading::AutoResetEvent(false);
		System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::KeyEventArgs ^> ^> ^_keyboardEvents = gcnew System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::KeyEventArgs ^> ^>();
	};
}