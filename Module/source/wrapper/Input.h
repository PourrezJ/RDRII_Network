#pragma once

void ManagedScriptKeyboardMessage(unsigned long key, unsigned short repeats, unsigned char scanCode, bool isExtended, bool isWithAlt, bool wasDownBefore, bool isUpNow);
void ManagedScriptMouseMessage(unsigned long key);

namespace RDRN_Module
{
	public ref class Input abstract sealed
	{
	public:
		static bool IsKeyPressed(System::Windows::Forms::Keys key);
		static void PauseKeyboardEvents(bool paused);
	internal:
		static bool _captureKeyboardEvents = true;
		static array<bool>^ _keyboardState = gcnew array<bool>(256);
	};
}
