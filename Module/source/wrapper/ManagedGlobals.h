#pragma unmanaged
#include "../core/core.hpp"
#pragma managed

#include "Script.h"
#include "ScriptDomain.h"

namespace RDRN_Module
{
	public ref class ManagedGlobals
	{
	public:
		static System::AppDomain^ g_appDomain;
		static RDRN_Module::ScriptDomain^ g_scriptDomain;

		static property System::IntPtr g_module {
			System::IntPtr get()
			{
				return (System::IntPtr)rh2::GetModule();
			}
		}
	};
}
