#include "InputArgument.h"

#include "NativeObjects.h"

RDRN_Module::Native::InputArgument::InputArgument(System::Object^ value)
{
	_data = EncodeObject(value);
}

System::String^ RDRN_Module::Native::InputArgument::ToString()
{
	return _data.ToString();
}

// Value types
RDRN_Module::Native::InputArgument::operator InputArgument ^ (bool value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (char value)
{
	return gcnew InputArgument(static_cast<int>(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (unsigned char value)
{
	return gcnew InputArgument(static_cast<int>(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (short value)
{
	return gcnew InputArgument(static_cast<int>(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (unsigned short value)
{
	return gcnew InputArgument(static_cast<int>(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (int value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (unsigned int value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (float value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (double value)
{
	return gcnew InputArgument(static_cast<float>(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (System::Enum^ value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (INativeValue^ value)
{
	return gcnew InputArgument(value);
}

// String types
RDRN_Module::Native::InputArgument::operator InputArgument ^ (System::String^ value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (const char* value)
{
	return gcnew InputArgument(gcnew System::String(value));
}

// Pointer types
RDRN_Module::Native::InputArgument::operator InputArgument ^ (System::IntPtr value)
{
	return gcnew InputArgument(value);
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (bool* value)
{
	return gcnew InputArgument(System::IntPtr(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (int* value)
{
	return gcnew InputArgument(System::IntPtr(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (unsigned int* value)
{
	return gcnew InputArgument(System::IntPtr(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (float* value)
{
	return gcnew InputArgument(System::IntPtr(value));
}

RDRN_Module::Native::InputArgument::operator InputArgument ^ (void* value)
{
	return gcnew InputArgument(System::IntPtr(value));
}