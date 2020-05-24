#pragma once
//#include "stdafx.h"
#include <windows.h>
#undef Yield
#include <WinBase.h>

namespace RDRN_Module
{
	public ref class Script abstract
	{
	internal:
		void* m_fiberMain;
		void* m_fiberCurrent;
		int m_fiberWait;
		System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::Keys>^>^ m_keyboardEvents = gcnew System::Collections::Concurrent::ConcurrentQueue<System::Tuple<bool, System::Windows::Forms::Keys>^>();
		System::Collections::Concurrent::ConcurrentQueue<System::Windows::Forms::MouseButtons>^ m_mouseEvents = gcnew System::Collections::Concurrent::ConcurrentQueue<System::Windows::Forms::MouseButtons>();

	public:
		Script();

		void Wait(int ms);
		void Yield();

		virtual void OnInit();
		virtual void OnTick();
		virtual void OnKeyDown(System::Windows::Forms::KeyEventArgs^ args);
		virtual void OnKeyUp(System::Windows::Forms::KeyEventArgs^ args);
		virtual void OnMouseDown(System::Windows::Forms::MouseButtons button);
		//virtual void OnMouseUp(System::Windows::Forms::MouseEventArgs^ args);

		static Script^ GetExecuting();
		static void WaitExecuting(int ms);
		static void YieldExecuting();
	internal:
		void ProcessOneTick();
	};
}
