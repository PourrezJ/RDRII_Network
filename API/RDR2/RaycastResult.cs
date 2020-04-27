using System;
using RDRN_Module.Math;
using RDRN_API.Native;
using RDRN_Module.Native;

namespace RDRN_API
{
	public struct RaycastResult
	{
		public RaycastResult(int handle) : this()
		{
			Vector3 hitPositionArg;
			bool hitSomethingArg;
			int entityHandleArg;
			Vector3 surfaceNormalArg;
			unsafe
			{
				Result = Function.Call<int>(Hash.GET_SHAPE_TEST_RESULT, handle, &hitSomethingArg, &hitPositionArg, &surfaceNormalArg, &entityHandleArg);
			}

			DidHit = hitSomethingArg;
			HitPosition = hitPositionArg;
			SurfaceNormal = surfaceNormalArg;
			HitEntity = Entity.FromHandle(entityHandleArg);
		}

		/// <summary>
		/// Gets the raycast result code.
		/// </summary>
		public int Result { get; private set; }

		/// <summary>
		/// Gets the <see cref="Entity" /> this raycast collided with.
		/// <remarks>Returns <c>null</c> if the raycast didn't collide with any <see cref="Entity"/>.</remarks>
		/// </summary>
		public Entity HitEntity { get; private set; }
		/// <summary>
		/// Gets the world coordinates where this raycast collided.
		/// <remarks>Returns <see cref="Vector3.Zero"/> if the raycast didn't collide with anything.</remarks>
		/// </summary>
		public Vector3 HitPosition { get; private set; }

		/// <summary>
		/// Gets the normal of the surface where this raycast collided.
		/// <remarks>Returns <see cref="Vector3.Zero"/> if the raycast didn't collide with anything.</remarks>
		/// </summary>
		public Vector3 SurfaceNormal { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this raycast collided with anything.
		/// </summary>
		public bool DidHit { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this raycast collided with any <see cref="Entity"/>.
		/// </summary>
		public bool DidHitEntity { get { return !ReferenceEquals(HitEntity, null); } }
	}

	[Flags]
	public enum IntersectOptions
	{
		Everything = -1,
		Map = 1,
		MissionEntities,
		Peds1 = 12,
		Objects = 16,
		Unk1 = 32,
		Unk2 = 64,
		Unk3 = 128,
		Vegetation = 256,
		Unk4 = 512
	}
}
