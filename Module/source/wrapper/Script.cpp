
#include "ScriptDomain.h"


using namespace System;
using namespace System::Threading;
using namespace System::Collections::Concurrent;
namespace WinForms = System::Windows::Forms;

namespace RDRN_Module
{
	extern void HandleUnhandledException(Object^ sender, UnhandledExceptionEventArgs^ args);

	Script::Script() : _filename(ScriptDomain::CurrentDomain->LookupScriptFilename(this)), _scriptdomain(ScriptDomain::CurrentDomain)
	{
	}

	int Script::Interval::get()
	{
		return _interval;
	}
	void Script::Interval::set(int value)
	{
		if (value < 0)
		{
			value = 0;
		}

		_interval = value;
	}

	void Script::Abort()
	{
		try
		{
			Aborted(this, EventArgs::Empty);
		}
		catch (Exception^ ex)
		{
			HandleUnhandledException(this, gcnew UnhandledExceptionEventArgs(ex, true));
		}

		_waitEvent->Set();

		_scriptdomain->AbortScript(this);
	}
	void Script::Wait(int ms)
	{
		Script^ script = ScriptDomain::ExecutingScript;
		
		if (Object::ReferenceEquals(script, nullptr) || !script->_running)
		{
			throw gcnew InvalidOperationException("Illegal call to 'Script.Wait()' outside main loop!");
		}

		const auto resume = DateTime::UtcNow + TimeSpan::FromMilliseconds(ms);

		do
		{
			script->_waitEvent->Set();
			script->_continueEvent->WaitOne();
		} while (DateTime::UtcNow < resume);
	}
	void Script::Yield()
	{
		Wait(0);
	}

	void Script::MainLoop()
	{
		// Wait for domain to run scripts
		_continueEvent->WaitOne();

		// Run main loop
		while (_running)
		{
			Tuple<bool, WinForms::KeyEventArgs^>^ keyevent = nullptr;

			// Process events
			while (_keyboardEvents->TryDequeue(keyevent))
			{
				try
				{
					if (keyevent->Item1)
					{
						KeyDown(this, keyevent->Item2);
					}
					else
					{
						KeyUp(this, keyevent->Item2);
					}
				}
				catch (Exception^ ex)
				{
					HandleUnhandledException(this, gcnew UnhandledExceptionEventArgs(ex, false));
					break;
				}
			}

			try
			{
				Tick(this, EventArgs::Empty);
			}
			catch (Exception^ ex)
			{
				HandleUnhandledException(this, gcnew UnhandledExceptionEventArgs(ex, true));

				Abort();
				break;
			}

			// Yield execution to next tick
			Wait(_interval);
		}
	}
}
