
#include <cstdio>
#include <cstdarg>

#pragma unmanaged
namespace rdrn_module {
	static void UnmanagedLogWrite(const char* format, ...)
	{
		FILE* fh = fopen("RDRN_Module.log", "ab");
		if (fh == nullptr) {
			return;
		}

		va_list va;
		va_start(va, format);
		vfprintf(fh, format, va);
		std::cout << format << va << std::endl;
		va_end(va);

		fclose(fh);
	}
}
