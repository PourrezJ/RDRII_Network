using Shared.Math;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RDRN_Core.Native
{
	public static class Function
	{
		//[DllImport("ScriptHookRDR2.dll", ExactSpelling = true, EntryPoint = "?nativeInit@@YAX_K@Z")]
		//static extern void NativeInit(ulong hash);

		//[DllImport("ScriptHookRDR2.dll", ExactSpelling = true, EntryPoint = "?nativePush64@@YAX_K@Z")]
		//static extern void NativePush64(ulong val);

		//[DllImport("ScriptHookRDR2.dll", ExactSpelling = true, EntryPoint = "?nativeCall@@YAPEA_KXZ")]
		//static unsafe extern ulong* NativeCall();

		[DllImport("RDRN_Module.dll", ExactSpelling = true, EntryPoint = "?_NativeInit@Invoker@rh2@@CAX_K@Z")]
		static extern void NativeInit(ulong hash);

		[DllImport("RDRN_Module.dll", ExactSpelling = true, EntryPoint = "?_NativePush@Invoker@rh2@@CAX_K@Z")]
		static extern void NativePush64(ulong val);

		[DllImport("RDRN_Module.dll", ExactSpelling = true, EntryPoint = "?NativeCall@Invoker@rh2@@SAPEA_KXZ")]
		static unsafe extern ulong* NativeCall();

		[DllImport("RDRN_Module.dll", ExactSpelling = true, EntryPoint = "?GetCommandHandler@Invoker@rh2@@SAP6AXPEAX@Z_K@Z")]
		internal static unsafe extern ulong* GetCommandHandler(Hash hash);

		private static object lockObj = new object();

		class NativeTask : IScriptTask
		{
			internal ulong Hash;
			internal ulong[] Arguments;
			internal unsafe ulong* Result;

			public unsafe void Run()
			{
				Result = InvokeInternal((Hash)Hash, Arguments);
			}
		}

		internal static unsafe ulong* InvokeInternal(Hash hash, params ulong[] args)
		{
			lock (lockObj)
			{
				NativeInit((ulong)hash);
				foreach (var arg in args)
					NativePush64(arg);
				return NativeCall();
			}
		}

		internal static T Call<T>(Hash hash, params InputArgument[] arguments)
		{
			ulong[] args = new ulong[arguments.Length];
			for (int i = 0; i < arguments.Length; ++i)
			{
				args[i] = arguments[i].data;
			}

			unsafe
			{

				var res = InvokeInternal(hash, args);

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

		internal static void Call(Hash hash, params InputArgument[] arguments)
		{
			ulong[] args = new ulong[arguments.Length];
			for (int i = 0; i < arguments.Length; ++i)
			{
				args[i] = arguments[i].data;
			}

			unsafe
			{
				InvokeInternal(hash, args);
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
				return (ulong)ScriptDomain.CurrentDomain.PinString(valueString).ToInt64();
				//return (ulong)RDRN_Module.Native.Func.AddStringPool(valueString).ToInt64();
				//return 0; //temp fix invoker
			}

			// Scripting types
			if (value is Control control)
			{
				return (ulong)control;
			}
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
			if (type == typeof(Control))
			{
				return (uint)value;
			}
			throw new InvalidCastException(string.Concat("Unable to cast native value to object of type '", type, "'"));
		}

		/// <summary>
		/// Pushes a single string component on the text stack.
		/// </summary>
		/// <param name="str">The string to push.</param>
		static void PushString(string str)
		{
			ulong[] conargs = ConvertPrimitiveArguments(new object[] { 10, "LITERAL_STRING", str });
			ScriptDomain.CurrentDomain.ExecuteTask(new NativeTask
			{
				Hash = 0xFA925AC00EB830B9,
				Arguments = conargs
			});
		}

		public static void PushLongString(string str)
		{
			PushLongString(str, PushString);
		}
		public static void PushLongString(string str, Action<string> action)
		{
			const int maxLengthUtf8 = 99;

			if (Encoding.UTF8.GetByteCount(str) <= maxLengthUtf8)
			{
				action(str);
				return;
			}

			int startPos = 0;
			int currentPos = 0;
			int currentUtf8StrLength = 0;

			while (currentPos < str.Length)
			{
				int codePointSize = 0;

				// Calculate the UTF-8 code point size of the current character
				var chr = str[currentPos];
				if (chr < 0x80)
				{
					codePointSize = 1;
				}
				else if (chr < 0x800)
				{
					codePointSize = 2;
				}
				else if (chr < 0x10000)
				{
					codePointSize = 3;
				}
				else
				{
					#region Surrogate check
					const int LowSurrogateStart = 0xD800;
					const int HighSurrogateStart = 0xD800;

					var temp1 = (int)chr - HighSurrogateStart;
					if (temp1 >= 0 && temp1 <= 0x7ff)
					{
						// Found a high surrogate
						if (currentPos < str.Length - 1)
						{
							var temp2 = str[currentPos + 1] - LowSurrogateStart;
							if (temp2 >= 0 && temp2 <= 0x3ff)
							{
								// Found a low surrogate
								codePointSize = 4;
							}
						}
					}
					#endregion
				}

				if (currentUtf8StrLength + codePointSize > maxLengthUtf8)
				{
					action(str.Substring(startPos, currentPos - startPos));

					startPos = currentPos;
					currentUtf8StrLength = 0;
				}
				else
				{
					currentPos++;
					currentUtf8StrLength += codePointSize;
				}

				// Additional increment is needed for surrogate
				if (codePointSize == 4)
				{
					currentPos++;
				}
			}

			action(str.Substring(startPos, str.Length - startPos));
		}

		/// Helper function that converts an array of primitive values to a native stack.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		static unsafe ulong[] ConvertPrimitiveArguments(object[] args)
		{
			var result = new ulong[args.Length];
			for (int i = 0; i < args.Length; ++i)
			{
				if (args[i] is bool valueBool)
				{
					result[i] = valueBool ? 1ul : 0ul;
					continue;
				}
				if (args[i] is byte valueByte)
				{
					result[i] = (ulong)valueByte;
					continue;
				}
				if (args[i] is int valueInt32)
				{
					result[i] = (ulong)valueInt32;
					continue;
				}
				if (args[i] is ulong valueUInt64)
				{
					result[i] = valueUInt64;
					continue;
				}
				if (args[i] is float valueFloat)
				{
					result[i] = *(ulong*)&valueFloat;
					continue;
				}
				if (args[i] is IntPtr valueIntPtr)
				{
					result[i] = (ulong)valueIntPtr.ToInt64();
					continue;
				}
				if (args[i] is string valueString)
				{
					result[i] = (ulong)ScriptDomain.CurrentDomain.PinString(valueString).ToInt64();
					continue;
				}
				if (args[i] is Control valueControl)
				{
					result[i] = (ulong)valueControl;
					continue;
				}
				throw new ArgumentException("Unknown primitive type in native argument list", nameof(args));
			}

			return result;
		}
	}
}
