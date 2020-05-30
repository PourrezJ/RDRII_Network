using RDRN_Module.Math;
using RDRN_Module.Native;
using System;
using System.Runtime.InteropServices;

namespace RDRN_API.Native
{
	public static class Function
	{
		private static object lockObj = new object();

		public static T Call<T>(Hash hash, params InputArgument[] arguments)
		{
			lock (lockObj)
			{
				ulong[] args = new ulong[arguments.Length];
				for (int i = 0; i < arguments.Length; ++i)
				{
					args[i] = arguments[i].data;
				}

				unsafe
				{
					
					var res = RDRN_Module.Native.Func.InvokeManaged(hash, args);

					// The result will be null when this method is called from a thread other than the main thread
					if (res == null)
					{
						throw new InvalidOperationException("Native.Function.Call can only be called from the main thread.");
					}

					if (typeof(T).IsEnum || typeof(T).IsPrimitive || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector2))
					{
						return ObjectFromNative<T>(res);
					}
					else
					{
						return (T)ObjectFromNative(typeof(T), res);
					}
				}
			}			
		}
		public static void Call(Hash hash, params InputArgument[] arguments)
		{
			lock (lockObj)
			{
				ulong[] args = new ulong[arguments.Length];
				for (int i = 0; i < arguments.Length; ++i)
				{
					args[i] = arguments[i].data;
				}

				unsafe
				{
					RDRN_Module.Native.Func.InvokeManaged(hash, args);
				}
			}
		}

		internal static unsafe ulong ObjectToNative(object value)
		{
			if (value is null)
			{
				return 0;
			}

			if (value is bool valueBool)
			{
				return valueBool ? 1ul : 0ul;
			}
			if (value is int valueInt32)
			{
				// Prevent value from changing memory expression, in case the type is incorrect
				return (uint)valueInt32;
			}
			if (value is uint valueUInt32)
			{
				return valueUInt32;
			}
			if (value is float valueFloat)
			{
				return *(uint*)&valueFloat;
			}
			if (value is double valueDouble)
			{
				valueFloat = (float)valueDouble;
				return *(uint*)&valueFloat;
			}
			if (value is IntPtr valueIntPtr)
			{
				return (ulong)valueIntPtr.ToInt64();
			}
			if (value is string valueString)
			{
				//return (ulong)SHVDN.ScriptDomain.CurrentDomain.PinString(valueString).ToInt64();
				//return (ulong)RDRN_Module.Native.Func.AddStringPool(valueString).ToInt64();
				return 0; //temp fix invoker
			}

			// Scripting types
			if (value is Model valueModel)
			{
				return (ulong)valueModel.Hash;
			}
			if (typeof(IHandleable).IsAssignableFrom(value.GetType()))
			{
				return (ulong)((IHandleable)value).Handle;
			}

			throw new InvalidCastException(string.Concat("Unable to cast object of type '", value.GetType(), "' to native value"));
		}

		internal static unsafe T ObjectFromNative<T>(ulong* value)
		{
			if (typeof(T).IsEnum)
			{
				return NativeHelper<T>.Convert(*value);
			}

			if (typeof(T) == typeof(bool))
			{
				// Return proper boolean values (true if non-zero and false if zero)
				bool valueBool = *value != 0;
				return NativeHelper<T>.PtrToStructure(new IntPtr(&valueBool));
			}

			if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(float))
			{
				return NativeHelper<T>.PtrToStructure(new IntPtr(value));
			}

			if (typeof(T) == typeof(double))
			{
				return NativeHelper<T>.Convert(NativeHelper<T>.PtrToStructure(new IntPtr(value)));
			}

			if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3))
			{
				return NativeHelper<T>.Convert(*(NativeVector3*)value);
			}

			throw new InvalidCastException(string.Concat("Unable to cast native value to object of type '", typeof(T), "'"));
		}

		internal static unsafe object ObjectFromNative(Type type, ulong* value)
		{
			if (type == typeof(string))
			{
				return NativeMemory.PtrToStringUTF8(new IntPtr((char*)*value));
			}

			// Scripting types
			if (type == typeof(Blip))
			{
				return new Blip(*(int*)value);
			}
			if (type == typeof(Camera))
			{
				return new Camera(*(int*)value);
			}
			if (type == typeof(Entity))
			{
				return Entity.FromHandle(*(int*)value);
			}
			if (type == typeof(Ped))
			{
				return new Ped(*(int*)value);
			}
			if (type == typeof(PedGroup))
			{
				return new PedGroup(*(int*)value);
			}
			if (type == typeof(Player))
			{
				return new Player(*(int*)value);
			}
			if (type == typeof(Prop))
			{
				return new Prop(*(int*)value);
			}
			if (type == typeof(Rope))
			{
				return new Rope(*(int*)value);
			}
			if (type == typeof(Vehicle))
			{
				return new Vehicle(*(int*)value);
			}

			throw new InvalidCastException(string.Concat("Unable to cast native value to object of type '", type, "'"));
		}
	}
}
