using System.Runtime.InteropServices;
using Shared.Math;

namespace RDRN_Core.Native
{
	[StructLayout(LayoutKind.Explicit, Size = 0x18)]
	internal struct NativeVector3
	{
		[FieldOffset(0x00)]
		internal float X;
		[FieldOffset(0x08)]
		internal float Y;
		[FieldOffset(0x10)]
		internal float Z;

		internal NativeVector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static explicit operator Vector2(NativeVector3 val) => new Vector2(val.X, val.Y);
		public static explicit operator Vector3(NativeVector3 val) => new Vector3(val.X, val.Y, val.Z);
	}
}