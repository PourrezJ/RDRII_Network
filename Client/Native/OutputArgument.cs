using Shared.Math;
using System;
using System.Runtime.InteropServices;

namespace RDRN_Core.Native
{
	public class OutputArgument : InputArgument, IDisposable
	{
		public OutputArgument() : base(Marshal.AllocCoTaskMem(24))
		{
		}
		public OutputArgument(object initvalue) : this()
		{
			unsafe { *(ulong*)data = Function.ObjectToNative(initvalue); }
		}

		public OutputArgument([MarshalAs(UnmanagedType.U1)] bool initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(byte initvalue) : this((object)(int)initvalue)
		{
		}
		public OutputArgument(sbyte initvalue) : this((object)(int)initvalue)
		{
		}
		public OutputArgument(short initvalue) : this((object)(int)initvalue)
		{
		}
		public OutputArgument(ushort initvalue) : this((object)(int)initvalue)
		{
		}
		public OutputArgument(int initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(uint initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(float initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(double initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(string initvalue) : this((object)initvalue)
		{
		}
		
		public OutputArgument(Model initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Blip initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Camera initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Entity initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Ped initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(PedGroup initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Player initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Prop initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Vehicle initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Rope initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Control initvalue) : this((object)initvalue)
		{
		}
		public OutputArgument(Relationship initvalue) : this((object)initvalue)
		{
		}

		~OutputArgument()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool disposing)
		{
			if (data != 0)
			{
				Marshal.FreeCoTaskMem((IntPtr)(long)data);
				data = 0;
			}
		}

		public T GetResult<T>()
		{
			unsafe
			{
				if (typeof(T).IsEnum || typeof(T).IsPrimitive || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector2))
				{
					return Function.ObjectFromNative<T>((ulong*)data);
				}
				else
				{
					return (T)Function.ObjectFromNative(typeof(T), (ulong*)data);
				}
			}
		}
	}
}
