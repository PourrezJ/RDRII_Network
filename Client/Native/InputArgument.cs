using System;
using System.Runtime.InteropServices;

namespace RDRN_Core.Native
{
	public class InputArgument
	{
		internal ulong data;

		public InputArgument(ulong value)
		{
			data = value;
		}
		public InputArgument(object value)
		{
			data = Function.ObjectToNative(value);
		}

		public InputArgument([MarshalAs(UnmanagedType.U1)] bool value) : this(value ? 1ul : 0ul)
		{
		}
		public InputArgument(int value) : this((uint)value)
		{
		}
		public InputArgument(uint value) : this((ulong)value)
		{
		}
		public InputArgument(float value)
		{
			unsafe
			{
				data = *(uint*)&value;
			}
		}
		public InputArgument(double value) : this((float)value)
		{
		}
		public InputArgument(string value) : this((object)value)
		{
		}
		public InputArgument(Model value) : this((uint)value.Hash)
		{
		}
		public InputArgument(Blip value) : this((object)value)
		{
		}
		public InputArgument(Camera value) : this((object)value)
		{
		}
		public InputArgument(Entity value) : this((object)value)
		{
		}
		public InputArgument(Ped value) : this((object)value)
		{
		}
		public InputArgument(PedGroup value) : this((object)value)
		{
		}
		public InputArgument(Player value) : this((object)value)
		{
		}
		public InputArgument(Prop value) : this((object)value)
		{
		}
		public InputArgument(Vehicle value) : this((object)value)
		{
		}
		public InputArgument(Rope value) : this((object)value)
		{
		}

		public InputArgument(Relationship value) : this((int)value)
		{
		}
		public InputArgument(UI.CursorSprite value) : this((int)value)
		{
		}
		public InputArgument(Control value) : this((uint)value)
		{
		}

		public static implicit operator InputArgument([MarshalAs(UnmanagedType.U1)] bool value)
		{
			return value ? new InputArgument(1ul) : new InputArgument(0ul);
		}
		public static implicit operator InputArgument(byte value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(sbyte value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(short value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(ushort value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(int value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(uint value)
		{
			return new InputArgument((ulong)value);
		}
		public static implicit operator InputArgument(float value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(double value)
		{
			// Native functions don't consider any arguments as double, so convert double values to float ones
			return new InputArgument((float)value);
		}
		public static implicit operator InputArgument(string value)
		{
			return new InputArgument(value);
		}

		public static unsafe implicit operator InputArgument(bool* value)
		{
			return new InputArgument((ulong)new IntPtr(value).ToInt64());
		}
		public static unsafe implicit operator InputArgument(int* value)
		{
			return new InputArgument((ulong)new IntPtr(value).ToInt64());
		}
		public static unsafe implicit operator InputArgument(uint* value)
		{
			return new InputArgument((ulong)new IntPtr(value).ToInt64());
		}
		public static unsafe implicit operator InputArgument(float* value)
		{
			return new InputArgument((ulong)new IntPtr(value).ToInt64());
		}
		public static unsafe implicit operator InputArgument(sbyte* value)
		{
			return new InputArgument(new string(value));
		}
		
		public static implicit operator InputArgument(Model value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Blip value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Camera value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Entity value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Ped value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(PedGroup value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Player value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Prop value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Vehicle value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Rope value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Control value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(Relationship value)
		{
			return new InputArgument(value);
		}
		public static implicit operator InputArgument(UI.CursorSprite value)
		{
			return new InputArgument(value);
		}
		public override string ToString()
		{
			return data.ToString();
		}
	}

}
