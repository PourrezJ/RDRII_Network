using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RDRN_Core
{
	[StructLayout(LayoutKind.Explicit)]
	struct GenericPool
	{
		[FieldOffset(0x00)]
		public ulong poolStartAddress;
		[FieldOffset(0x08)]
		public IntPtr byteArray;
		[FieldOffset(0x10)]
		public uint size;
		[FieldOffset(0x14)]
		public uint itemSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValid(uint index)
		{
			return Mask(index) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ulong GetAddress(uint index)
		{
			return ((Mask(index) & (poolStartAddress + index * itemSize)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Mask(uint index)
		{
			unsafe
			{
				byte* byteArrayPtr = (byte*)byteArray.ToPointer();
				long num1 = byteArrayPtr[index] & 0x80;
				return (ulong)(~((num1 | -num1) >> 63));
			}
		}
	}

	internal static class Memory
	{

		internal static unsafe byte* FindPattern(string pattern, string mask)
		{
			ProcessModule module = Process.GetCurrentProcess().MainModule;

			ulong address = (ulong)module.BaseAddress.ToInt64();
			ulong endAddress = address + (ulong)module.ModuleMemorySize;

			for (; address < endAddress; address++)
			{
				for (int i = 0; i < pattern.Length; i++)
				{
					if (mask[i] != '?' && ((byte*)address)[i] != pattern[i])
						break;
					else if (i + 1 == pattern.Length)
						return (byte*)address;
				}
			}

			return null;
		}
	}
}
