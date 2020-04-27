#include "OutputArgument.h"

#include "NativeObjects.h"

#include <cstring>
#include <cstdlib>

RDRN_Module::Native::OutputArgument::OutputArgument()
{
	_storage = malloc(24);
	memset(_storage, 0, 24);
}

RDRN_Module::Native::OutputArgument::OutputArgument(Object^ value) : OutputArgument()
{
	*reinterpret_cast<System::UInt64*>(_storage) = EncodeObject(value);
}

RDRN_Module::Native::OutputArgument::~OutputArgument()
{
	this->!OutputArgument();
}

generic <typename T> T RDRN_Module::Native::OutputArgument::GetResult()
{
	return static_cast<T>(DecodeObject(T::typeid, reinterpret_cast<System::UInt64*>(_storage)));
}

void* RDRN_Module::Native::OutputArgument::GetPointer()
{
	return _storage;
}

RDRN_Module::Native::OutputArgument::!OutputArgument()
{
	free(_storage);
}