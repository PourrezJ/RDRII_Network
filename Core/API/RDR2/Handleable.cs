//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using Shared.Math;

namespace RDRN_Core
{
	/*public interface ISpatial
	{
		Vector3 Position
		{
			get; set;
		}
		Vector3 Rotation
		{
			get; set;
		}
	}*/

	public interface IHandleable
	{
		int Handle { get; }

		bool Exists();
	}
}
