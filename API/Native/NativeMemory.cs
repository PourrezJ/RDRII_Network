using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.Marshal;

namespace RDRN_API.Native
{
	public static unsafe class NativeMemory
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

		static NativeMemory()
		{
			byte* address;

			// Get relative address and add it to the instruction address.
			/*
			address = FindPattern("\x74\x21\x48\x8B\x48\x20\x48\x85\xC9\x74\x18\x48\x8B\xD6\xE8", "xxxxxxxxxxxxxxx") - 10;
			GetPtfxAddressFunc = GetDelegateForFunctionPointer<GetHandleAddressFuncDelegate>(
				new IntPtr(*(int*)(address) + address + 4));

			address = FindPattern("\xE8\x00\x00\x00\x00\x48\x8B\xD8\x48\x85\xC0\x74\x2E\x48\x83\x3D", "x????xxxxxxxxxxx");
			GetEntityAddressFunc = GetDelegateForFunctionPointer<GetHandleAddressFuncDelegate>(
				new IntPtr(*(int*)(address + 1) + address + 5));

			address = FindPattern("\xB2\x01\xE8\x00\x00\x00\x00\x48\x85\xC0\x74\x1C\x8A\x88", "xxx????xxxxxxx");
			GetPlayerAddressFunc = GetDelegateForFunctionPointer<GetHandleAddressFuncDelegate>(
				new IntPtr(*(int*)(address + 3) + address + 7));

			address = FindPattern("\x48\xF7\xF9\x49\x8B\x48\x08\x48\x63\xD0\xC1\xE0\x08\x0F\xB6\x1C\x11\x03\xD8", "xxxxxxxxxxxxxxxxxxx");
			AddEntityToPoolFunc = GetDelegateForFunctionPointer<AddEntityToPoolFuncDelegate>(
				new IntPtr(address - 0x68));

			address = FindPattern("\x48\x8B\xDA\xE8\x00\x00\x00\x00\xF3\x0F\x10\x44\x24", "xxxx????xxxxx");
			EntityPosFunc = GetDelegateForFunctionPointer<EntityPosFuncDelegate>(
				new IntPtr((address - 6)));

			address = FindPattern("\x0F\x85\x00\x00\x00\x00\x48\x8B\x4B\x20\xE8\x00\x00\x00\x00\x48\x8B\xC8", "xx????xxxxx????xxx");
			EntityModel1Func = GetDelegateForFunctionPointer<EntityModel1FuncDelegate>(
				new IntPtr((*(int*)address + 11) + address + 15));

			address = FindPattern("\x45\x33\xC9\x3B\x05", "xxxxx");
			EntityModel2Func = GetDelegateForFunctionPointer<EntityModel2FuncDelegate>(
				new IntPtr(address - 0x46));

			*/
			// Find entity pools
			address = FindPattern("\x48\x89\x5c\x24\x00\x48\x89\x6c\x24\x00\x48\x89\x74\x24\x00\x57\x48\x83\xec\x00\x8b\x15\x00\x00\x00\x00\x48\x8b\xf1\x48\x83\xc1\x00\x33\xff", "xxxx?xxxx?xxxx?xxxx?xx????xxxxxx?xx");
			PedPoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);

			Console.WriteLine("===================> Ped Pool Address <==================");
			/*
			
			address = FindPattern("\x48\x8B\x05\x00\x00\x00\x00\x8B\x78\x10\x85\xFF", "xxx????xxxxx");
			ObjectPoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);

			address = FindPattern("\x4C\x8B\x0D\x00\x00\x00\x00\x44\x8B\xC1\x49\x8B\x41\x08", "xxx????xxxxxxx");
			EntityPoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);

			address = FindPattern("\x48\x8B\x05\x00\x00\x00\x00\xF3\x0F\x59\xF6\x48\x8B\x08", "xxx????xxxxxxx");
			VehiclePoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);

			address = FindPattern("\x4C\x8B\x05\x00\x00\x00\x00\x40\x8A\xF2\x8B\xE9", "xxx????xxxxx");
			PickupObjectPoolAddress = (ulong*)(*(int*)(address + 3) + address + 7);
			*/
			/*
			address = FindPattern("\x84\xC0\x74\x34\x48\x8D\x0D\x00\x00\x00\x00\x48\x8B\xD3", "xxxxxxx????xxx");
			GetLabelTextByHashAddress = (ulong)(*(int*)(address + 7) + address + 11);

			address = FindPattern("\x48\x89\x5C\x24\x08\x48\x89\x6C\x24\x18\x89\x54\x24\x10\x56\x57\x41\x56\x48\x83\xEC\x20", "xxxxxxxxxxxxxxxxxxxxxx");
			GetLabelTextByHashFunc = GetDelegateForFunctionPointer<GetLabelTextByHashFuncDelegate>(new IntPtr(address));

			address = FindPattern("\x8A\x4C\x24\x60\x8B\x50\x10\x44\x8A\xCE", "xxxxxxxxxx");
			CheckpointPoolAddress = (ulong*)(*(int*)(address + 17) + address + 21);
			GetCheckpointBaseAddress = GetDelegateForFunctionPointer<GetCheckpointBaseAddressDelegate>(new IntPtr(*(int*)(address - 19) + address - 15));
			GetCheckpointHandleAddress = GetDelegateForFunctionPointer<GetCheckpointHandleAddressDelegate>(new IntPtr(*(int*)(address - 9) + address - 5));

			address = FindPattern("\x48\x8B\x0B\x33\xD2\xE8\x00\x00\x00\x00\x89\x03", "xxxxxx????xx");
			GetHashKeyFunc = GetDelegateForFunctionPointer<GetHashKeyDelegate>(new IntPtr(*(int*)(address + 6) + address + 10));

			address = FindPattern("\x74\x11\x8B\xD1\x48\x8D\x0D\x00\x00\x00\x00\x45\x33\xC0", "xxxxxxx????xxx");
			cursorSpriteAddr = (int*)(*(int*)(address - 4) + address);

			address = FindPattern("\x48\x63\xC1\x48\x8D\x0D\x00\x00\x00\x00\xF3\x0F\x10\x04\x81\xF3\x0F\x11\x05\x00\x00\x00\x00", "xxxxxx????xxxxxxxxx????");
			readWorldGravityAddress = (float*)(*(int*)(address + 19) + address + 23);
			writeWorldGravityAddress = (float*)(*(int*)(address + 6) + address + 10);

			address = FindPattern("\xF3\x0F\x10\x0D\x00\x00\x00\x00\x41\x0F\x2F\xCB\x0F\x83", "xxxx????xxxxxx");
			var timeScaleArrayAddress = (float*)(*(int*)(address + 4) + address + 8);
			if (timeScaleArrayAddress != null)
				// SET_TIME_SCALE changes the 3rd element, so obtain the address of it
				timeScaleAddress = timeScaleArrayAddress + 2;

			// Find camera objects
			address = FindPattern("\x48\x8B\xC8\xEB\x02\x33\xC9\x48\x85\xC9\x74\x26", "xxxxxxxxxxxx") - 9;
			CameraPoolAddress = (ulong*)(*(int*)(address) + address + 4);
			address = FindPattern("\x48\x8B\xC7\xF3\x0F\x10\x0D", "xxxxxxx") - 0x1D;
			address = address + *(int*)(address) + 4;
			GameplayCameraAddress = (ulong*)(*(int*)(address + 3) + address + 7);
			*/
		}

		/// <summary>
		/// Reads a single 8-bit value from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The value at the address.</returns>
		public static byte ReadByte(IntPtr address)
		{
			return *(byte*)address.ToPointer();
		}
		/// <summary>
		/// Reads a single 16-bit value from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The value at the address.</returns>
		public static Int16 ReadInt16(IntPtr address)
		{
			return *(short*)address.ToPointer();
		}
		/// <summary>
		/// Reads a single 32-bit value from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The value at the address.</returns>
		public static Int32 ReadInt32(IntPtr address)
		{
			return *(int*)address.ToPointer();
		}
		/// <summary>
		/// Reads a single floating-point value from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The value at the address.</returns>
		public static float ReadFloat(IntPtr address)
		{
			return *(float*)address.ToPointer();
		}
		/// <summary>
		/// Reads a null-terminated UTF-8 string from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The string at the address.</returns>
		public static String ReadString(IntPtr address)
		{
			return PtrToStringUTF8(address);
		}
		/// <summary>
		/// Reads a single 64-bit value from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>The value at the address.</returns>
		public static IntPtr ReadAddress(IntPtr address)
		{
			return new IntPtr(*(void**)(address.ToPointer()));
		}
		/// <summary>
		/// Reads a 4x4 floating-point matrix from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>All elements of the matrix in row major arrangement.</returns>
		public static float[] ReadMatrix(IntPtr address)
		{
			var data = (float*)address.ToPointer();
			return new float[16] { data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15] };
		}
		/// <summary>
		/// Reads a 3-component floating-point vector from the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <returns>All elements of the vector.</returns>
		public static float[] ReadVector3(IntPtr address)
		{
			var data = (float*)address.ToPointer();
			return new float[3] { data[0], data[1], data[2] };
		}

		/// <summary>
		/// Writes a single 8-bit value to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The value to write.</param>
		public static void WriteByte(IntPtr address, byte value)
		{
			var data = (byte*)address.ToPointer();
			*data = value;
		}
		/// <summary>
		/// Writes a single 16-bit value to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The value to write.</param>
		public static void WriteInt16(IntPtr address, Int16 value)
		{
			var data = (short*)address.ToPointer();
			*data = value;
		}
		/// <summary>
		/// Writes a single 32-bit value to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The value to write.</param>
		public static void WriteInt32(IntPtr address, Int32 value)
		{
			var data = (int*)address.ToPointer();
			*data = value;
		}
		/// <summary>
		/// Writes a single floating-point value to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The value to write.</param>
		public static void WriteFloat(IntPtr address, float value)
		{
			var data = (float*)address.ToPointer();
			*data = value;
		}
		/// <summary>
		/// Writes a 4x4 floating-point matrix to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The elements of the matrix in row major arrangement to write.</param>
		public static void WriteMatrix(IntPtr address, float[] value)
		{
			var data = (float*)(address.ToPointer());
			for (int i = 0; i < value.Length; i++)
				data[i] = value[i];
		}
		/// <summary>
		/// Writes a 3-component floating-point to the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="value">The vector components to write.</param>
		public static void WriteVector3(IntPtr address, float[] value)
		{
			var data = (float*)address.ToPointer();
			data[0] = value[0];
			data[1] = value[1];
			data[2] = value[2];
		}

		/// <summary>
		/// Sets a single bit in the 32-bit value at the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="bit">The bit index to change.</param>
		public static void SetBit(IntPtr address, int bit)
		{
			if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException(nameof(bit), "The bit index has to be between 0 and 31");

			var data = (int*)address.ToPointer();
			*data |= (1 << bit);
		}
		/// <summary>
		/// Clears a single bit in the 32-bit value at the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="bit">The bit index to change.</param>
		public static void ClearBit(IntPtr address, int bit)
		{
			if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException(nameof(bit), "The bit index has to be between 0 and 31");

			var data = (int*)address.ToPointer();
			*data &= ~(1 << bit);
		}
		/// <summary>
		/// Checks a single bit in the 32-bit value at the specified <paramref name="address"/>.
		/// </summary>
		/// <param name="address">The memory address to access.</param>
		/// <param name="bit">The bit index to check.</param>
		/// <returns><c>true</c> if the bit is set, <c>false</c> if it is unset.</returns>
		public static bool IsBitSet(IntPtr address, int bit)
		{
			if (bit < 0 || bit > 31)
				throw new ArgumentOutOfRangeException(nameof(bit), "The bit index has to be between 0 and 31");

			var data = (int*)address.ToPointer();
			return (*data & (1 << bit)) != 0;
		}

		public static IntPtr String => StringToCoTaskMemUTF8("STRING");
		public static IntPtr NullString => StringToCoTaskMemUTF8(string.Empty);
		public static IntPtr CellEmailBcon => StringToCoTaskMemUTF8("CELL_EMAIL_BCON");

		public static string PtrToStringUTF8(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				return string.Empty;

			var data = (byte*)ptr.ToPointer();

			// Calculate length of null-terminated string
			int len = 0;
			while (data[len] != 0)
				++len;

			return PtrToStringUTF8(ptr, len);
		}
		public static string PtrToStringUTF8(IntPtr ptr, int len)
		{
			if (len < 0)
				throw new ArgumentException(null, nameof(len));

			if (ptr == IntPtr.Zero)
				return null;
			if (len == 0)
				return string.Empty;

			return Encoding.UTF8.GetString((byte*)ptr.ToPointer(), len);
		}
		public static IntPtr StringToCoTaskMemUTF8(string s)
		{
			if (s == null)
				return IntPtr.Zero;

			byte[] utf8Bytes = Encoding.UTF8.GetBytes(s);
			IntPtr dest = AllocCoTaskMem(utf8Bytes.Length + 1);
			if (dest == IntPtr.Zero)
				throw new OutOfMemoryException();

			Copy(utf8Bytes, 0, dest, utf8Bytes.Length);
			// Add null-terminator to end
			((byte*)dest.ToPointer())[utf8Bytes.Length] = 0;

			return dest;
		}

		#region -- Cameras --

		static ulong* CameraPoolAddress;
		static ulong* GameplayCameraAddress;

		public static IntPtr GetCameraAddress(int handle)
		{
			uint index = (uint)(handle >> 8);
			ulong poolAddr = *CameraPoolAddress;
			if (*(byte*)(index + *(long*)(poolAddr + 8)) == (byte)(handle & 0xFF))
			{
				return new IntPtr(*(long*)poolAddr + (index * *(uint*)(poolAddr + 20)));
			}
			return IntPtr.Zero;

		}
		public static IntPtr GetGameplayCameraAddress()
		{
			return new IntPtr((long)*GameplayCameraAddress);
		}

		#endregion

		#region -- Game Data --

		delegate uint GetHashKeyDelegate(IntPtr stringPtr, uint initialHash);
		static GetHashKeyDelegate GetHashKeyFunc;

		public static uint GetHashKey(string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key.ToLowerInvariant());

			uint num = 0u;

			for (int i = 0; i < bytes.Length; i++)
			{
				num += (uint)bytes[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;

			return (uint)(num + (num << 15));

			/*
			IntPtr keyPtr = ScriptDomain.CurrentDomain.PinString(key);
			return GetHashKeyFunc(keyPtr, 0);*/
		}

		static ulong GetLabelTextByHashAddress;
		delegate ulong GetLabelTextByHashFuncDelegate(ulong address, int labelHash);
		static GetLabelTextByHashFuncDelegate GetLabelTextByHashFunc;

		public static string GetGXTEntryByHash(int entryLabelHash)
		{
			var entryText = (char*)GetLabelTextByHashFunc(GetLabelTextByHashAddress, entryLabelHash);
			return entryText != null ? PtrToStringUTF8(new IntPtr(entryText)) : string.Empty;
		}

		#endregion

		#region -- World Data --

		static int* cursorSpriteAddr;

		public static int CursorSprite
		{
			get { return *cursorSpriteAddr; }
		}

		static float* timeScaleAddress;

		public static float TimeScale
		{
			get { return *timeScaleAddress; }
		}

		static float* readWorldGravityAddress;
		static float* writeWorldGravityAddress;

		public static float WorldGravity
		{
			get { return *readWorldGravityAddress; }
			set { *writeWorldGravityAddress = value; }
		}

		#endregion

		#region -- Entity Pools --

		[StructLayout(LayoutKind.Sequential)]
		struct Checkpoint
		{
			internal long padding;
			internal int padding1;
			internal int handle;
			internal long padding2;
			internal Checkpoint* next;
		}

		[StructLayout(LayoutKind.Explicit)]
		struct EntityPool
		{
			[FieldOffset(0x10)]
			internal uint num1;
			[FieldOffset(0x20)]
			internal uint num2;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal bool IsFull()
			{
				return num1 - (num2 & 0x3FFFFFFF) <= 256;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		struct VehiclePool
		{
			[FieldOffset(0x00)]
			internal ulong* poolAddress;
			[FieldOffset(0x08)]
			internal uint size;
			[FieldOffset(0x30)]
			internal uint* bitArray;
			[FieldOffset(0x60)]
			internal uint itemCount;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal bool IsValid(uint i)
			{
				return ((bitArray[i >> 5] >> ((int)i & 0x1F)) & 1) != 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal ulong GetAddress(uint i)
			{
				return poolAddress[i];
			}
		}

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

		static ulong* PedPoolAddress;
		static ulong* EntityPoolAddress;
		static ulong* ObjectPoolAddress;
		static ulong* PickupObjectPoolAddress;
		static ulong* VehiclePoolAddress;
		static ulong* CheckpointPoolAddress;

		delegate ulong EntityPosFuncDelegate(ulong address, float* position);
		delegate ulong EntityModel1FuncDelegate(ulong address);
		delegate ulong EntityModel2FuncDelegate(ulong address);
		delegate int AddEntityToPoolFuncDelegate(ulong address);

		static EntityPosFuncDelegate EntityPosFunc;
		static EntityModel1FuncDelegate EntityModel1Func;
		static EntityModel2FuncDelegate EntityModel2Func;
		static AddEntityToPoolFuncDelegate AddEntityToPoolFunc;

		internal class EntityPoolTask /*: IScriptTask*/
		{
			#region Fields
			internal Type poolType;
			internal List<int> handles = new List<int>();
			internal bool doPosCheck;
			internal bool doModelCheck;
			internal int[] modelHashes;
			internal float radiusSquared;
			internal float[] position;
			#endregion

			internal enum Type
			{
				Ped = 1,
				Object = 2,
				Vehicle = 4,
				PickupObject = 8
			}

			internal EntityPoolTask(Type type)
			{
				poolType = type;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool CheckEntity(ulong address)
			{


				return true;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool CheckCheckpoint(ulong address)
			{
				if (address == 0)
					return false;

				if (doPosCheck)
				{
					float* position = (float*)(address + 0x90);

					float x = this.position[0] - position[0];
					float y = this.position[1] - position[1];
					float z = this.position[2] - position[2];
					float distanceSquared = (x * x) + (y * y) + (z * z);
					if (distanceSquared > radiusSquared)
						return false;
				}

				return true;
			}

			public void Run()
			{
				if (*NativeMemory.EntityPoolAddress == 0)
					return;

				EntityPool* entityPool = (EntityPool*)(*NativeMemory.EntityPoolAddress);

				if (poolType.HasFlag(Type.Vehicle) && *NativeMemory.VehiclePoolAddress != 0)
				{
					VehiclePool* vehiclePool = *(VehiclePool**)(*NativeMemory.VehiclePoolAddress);

					for (uint i = 0; i < vehiclePool->size; i++)
					{
						if (entityPool->IsFull())
							break;

						if (vehiclePool->IsValid(i))
						{
							ulong address = vehiclePool->GetAddress(i);
							if (CheckEntity(address))
								handles.Add(NativeMemory.AddEntityToPoolFunc(address));
						}
					}
				}

				if (poolType.HasFlag(Type.Ped) && *NativeMemory.PedPoolAddress != 0)
				{
					GenericPool* pedPool = (GenericPool*)(*NativeMemory.PedPoolAddress);

					for (uint i = 0; i < pedPool->size; i++)
					{
						if (entityPool->IsFull())
							break;

						if (pedPool->IsValid(i))
						{
							ulong address = pedPool->GetAddress(i);
							if (CheckEntity(address))
								handles.Add(NativeMemory.AddEntityToPoolFunc(address));
						}
					}
				}

				if (poolType.HasFlag(Type.Object) && *NativeMemory.ObjectPoolAddress != 0)
				{
					GenericPool* propPool = (GenericPool*)(*NativeMemory.ObjectPoolAddress);

					for (uint i = 0; i < propPool->size; i++)
					{
						if (entityPool->IsFull())
							break;

						if (propPool->IsValid(i))
						{
							ulong address = propPool->GetAddress(i);
							if (CheckEntity(address))
								handles.Add(NativeMemory.AddEntityToPoolFunc(address));
						}
					}
				}

				if (poolType.HasFlag(Type.PickupObject) && *NativeMemory.PickupObjectPoolAddress != 0)
				{
					GenericPool* pickupPool = (GenericPool*)(*NativeMemory.PickupObjectPoolAddress);

					for (uint i = 0; i < pickupPool->size; i++)
					{
						if (entityPool->IsFull())
							break;

						if (pickupPool->IsValid(i))
						{
							ulong address = pickupPool->GetAddress(i);
							if (CheckCheckpoint(address))
								handles.Add(NativeMemory.AddEntityToPoolFunc(address));
						}
					}
				}
			}
		}

		public static int GetPedCount()
		{
			if (*PedPoolAddress != 0)
			{
				GenericPool* pool = (GenericPool*)(*PedPoolAddress);
				return (int)pool->itemSize;
			}
			return 0;
		}
		public static int GetVehicleCount()
		{
			if (*VehiclePoolAddress != 0)
			{
				VehiclePool* pool = *(VehiclePool**)(*VehiclePoolAddress);
				return (int)pool->itemCount;
			}
			return 0;
		}

		public static int[] GetPedHandles(int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Ped);
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			////ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}
		public static int[] GetPedHandles(float[] position, float radius, int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Ped);
			task.position = position;
			task.radiusSquared = radius * radius;
			task.doPosCheck = true;
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}

		public static int[] GetPropHandles(int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Object);
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}
		public static int[] GetPropHandles(float[] position, float radius, int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Object);
			task.position = position;
			task.radiusSquared = radius * radius;
			task.doPosCheck = true;
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}

		public static int[] GetEntityHandles()
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Ped | EntityPoolTask.Type.Object | EntityPoolTask.Type.Vehicle);

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}
		public static int[] GetEntityHandles(float[] position, float radius)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Ped | EntityPoolTask.Type.Object | EntityPoolTask.Type.Vehicle);
			task.position = position;
			task.radiusSquared = radius * radius;
			task.doPosCheck = true;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}

		public static int[] GetVehicleHandles(int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Vehicle);
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}
		public static int[] GetVehicleHandles(float[] position, float radius, int[] modelHashes = null)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.Vehicle);
			task.position = position;
			task.radiusSquared = radius * radius;
			task.doPosCheck = true;
			task.modelHashes = modelHashes;
			task.doModelCheck = modelHashes != null && modelHashes.Length > 0;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}

		public static int[] GetCheckpointHandles()
		{
			int[] handles = new int[64];

			ulong count = 0;
			for (Checkpoint* item = *(Checkpoint**)(GetCheckpointBaseAddress() + 48); item != null && count < 64; item = item->next)
			{
				handles[count++] = item->handle;
			}

			int[] dataArray = new int[count];
			unsafe
			{
				fixed (int* ptrBuffer = &dataArray[0])
				{
					Copy(handles, 0, new IntPtr(ptrBuffer), (int)count);
				}
			}
			return dataArray;
		}
		public static int[] GetPickupObjectHandles()
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.PickupObject);

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}
		public static int[] GetPickupObjectHandles(float[] position, float radius)
		{
			var task = new EntityPoolTask(EntityPoolTask.Type.PickupObject);
			task.position = position;
			task.radiusSquared = radius * radius;
			task.doPosCheck = true;

			//ScriptDomain.CurrentDomain.ExecuteTask(task);

			return task.handles.ToArray();
		}

		#endregion

		#region -- Entity Addresses --

		delegate ulong GetHandleAddressFuncDelegate(int handle);
		//static GetHandleAddressFuncDelegate GetPtfxAddressFunc;
		static GetHandleAddressFuncDelegate GetEntityAddressFunc;
		static GetHandleAddressFuncDelegate GetPlayerAddressFunc;
		/*
		public static IntPtr GetPtfxAddress(int handle)
		{
			return new IntPtr((long)GetPtfxAddressFunc(handle));
		}*/
		public static IntPtr GetEntityAddress(int handle)
		{
			return new IntPtr((long)GetEntityAddressFunc(handle));
		}
		public static IntPtr GetPlayerAddress(int handle)
		{
			return new IntPtr((long)GetPlayerAddressFunc(handle));
		}

		delegate ulong GetCheckpointBaseAddressDelegate();
		static GetCheckpointBaseAddressDelegate GetCheckpointBaseAddress;
		delegate ulong GetCheckpointHandleAddressDelegate(ulong baseAddr, int handle);
		static GetCheckpointHandleAddressDelegate GetCheckpointHandleAddress;

		public static IntPtr GetCheckpointAddress(int handle)
		{
			var addr = GetCheckpointHandleAddress(GetCheckpointBaseAddress(), handle);
			if (addr == 0) return IntPtr.Zero;
			return new IntPtr((long)((ulong)(CheckpointPoolAddress) + 96 * ((ulong)*(int*)(addr + 16))));
		}

		#endregion
	}
}