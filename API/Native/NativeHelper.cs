using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace RDRN_API.Native
{
	internal unsafe static class NativeHelper<T>
	{
		static class CastCache<TFrom>
		{
			internal static readonly Func<TFrom, T> Convert;

			static CastCache()
			{
				var paramExp = Expression.Parameter(typeof(TFrom));
				var convertExp = Expression.Convert(paramExp, typeof(T));
				Convert = Expression.Lambda<Func<TFrom, T>>(convertExp, paramExp).Compile();
			}
		}

		static readonly Func<IntPtr, T> _ptrToStrFunc;

		static NativeHelper()
		{
			var ptrToStrMethod = new DynamicMethod("PtrToStructure<" + typeof(T) + ">", typeof(T),
				new Type[] { typeof(IntPtr) }, typeof(NativeHelper<T>), true);

			ILGenerator generator = ptrToStrMethod.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldobj, typeof(T));
			generator.Emit(OpCodes.Ret);

			_ptrToStrFunc = (Func<IntPtr, T>)ptrToStrMethod.CreateDelegate(typeof(Func<IntPtr, T>));
		}

		internal static T Convert<TFrom>(TFrom from)
		{
			return CastCache<TFrom>.Convert(from);
		}

		internal static T PtrToStructure(IntPtr ptr)
		{
			return _ptrToStrFunc(ptr);
		}
	}
}
