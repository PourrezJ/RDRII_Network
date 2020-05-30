#include <Input.h>

#include <ManagedGlobals.h>
#include <Log.h>

#include <Windows.h>

void ManagedScriptKeyboardMessage(unsigned long key, unsigned short repeats, unsigned char scanCode, bool isExtended, bool isWithAlt, bool wasDownBefore, bool isUpNow)
{
	if (RDRN_Module::ManagedGlobals::g_scriptDomain == nullptr) {
		return;
	}

	if (key >= (unsigned long)RDRN_Module::Input::_keyboardState->Length) {
		return;
	}

	bool ctrl = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
	bool shift = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;
	bool status = !isUpNow;

	RDRN_Module::Input::_keyboardState[key] = status;

	if (RDRN_Module::Input::_captureKeyboardEvents) {
		auto wfkey = (System::Windows::Forms::Keys)key;
		if (ctrl) {
			wfkey = wfkey | System::Windows::Forms::Keys::Control;
		}
		if (shift) {
			wfkey = wfkey | System::Windows::Forms::Keys::Shift;
		}
		if (isWithAlt) {
			wfkey = wfkey | System::Windows::Forms::Keys::Alt;
		}

		auto eventinfo = gcnew System::Tuple<bool, System::Windows::Forms::Keys>(status, wfkey);

		RDRN_Module::ManagedGlobals::g_scriptDomain->QueueKeyboardEvent(eventinfo);
	}

	//TODO: API for scancodes or WM_CHAR text input?
}

bool RDRN_Module::Input::IsKeyPressed(System::Windows::Forms::Keys key)
{
	return _keyboardState[(int)key];
}

void RDRN_Module::Input::PauseKeyboardEvents(bool paused)
{
	RDRN_Module::Input::_captureKeyboardEvents = !paused;
}
