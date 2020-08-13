using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RDRN_Core;
using RDRN_Core.Native;
using RDRN_Core.Javascript;
using RDRN_Core.Misc;
using RDRN_Core.Util;
using RDRN_Core.Streamer;
using Shared;
using Vector3 = Shared.Math.Vector3;
using WeaponHash = RDRN_Core.WeaponHash;
using VehicleHash = RDRN_Core.Native.VehicleHash;

namespace RDRN_Core.Sync
{
    public partial class SyncPed
    {
        struct interpolation
        {
            internal Vector3 vecStart;
            internal Vector3 vecTarget;
            internal Vector3 vecError;
            internal long StartTime;
            internal long FinishTime;
            internal float LastAlpha;
        }

        private interpolation currentInterop = new interpolation();

        internal void StartInterpolation()
        {
            currentInterop = new interpolation();

            if (_isInVehicle)
            {
                if (_lastPosition == null) return;
                //if (Main.VehicleLagCompensation)
                //{

                    var dir = Position - _lastPosition;
                    currentInterop.vecTarget = Position + dir;
                    currentInterop.vecError = dir;
                    //MainVehicle == null ? dir : MainVehicle.Position - currentInterop.vecTarget;
                    //currentInterop.vecError *= Util.Lerp(0.25f, Util.Unlerp(100, 100, 400), 1f);
                //}
                //else
                //{
                //    var dir = Position - _lastPosition.Value;
                //    currentInterop.vecTarget = Position;
                //    currentInterop.vecError = dir;
                //    currentInterop.vecError *= Util.Util.Lerp(0.25f, Util.Util.Unlerp(100, 100, 400), 1f);
                //}

                if (MainVehicle != null)
                    currentInterop.vecStart = MainVehicle.Position;
            }
            else
            {
                //if (Main.OnFootLagCompensation)
                //{
                    var dir = Position - _lastPosition;
                    currentInterop.vecTarget = Position; // + dir;
                    currentInterop.vecError = dir ?? new Vector3();
                    currentInterop.vecStart = Position;

                    //MainVehicle == null ? dir : MainVehicle.Position - currentInterop.vecTarget;
                    //currentInterop.vecError *= Util.Lerp(0.25f, Util.Unlerp(100, 100, 400), 1f);
                //}
                //else
                //{
                //    var dir = Position - _lastPosition;

                //    currentInterop.vecTarget = Position;
                //    currentInterop.vecError = dir ?? new Vector3();
                //    currentInterop.vecError *= Util.Util.Lerp(0.25f, Util.Util.Unlerp(100, 100, 400), 1f);
                //}

                if (Character != null)
                    currentInterop.vecStart = Character.Position;
            }

            currentInterop.StartTime = Util.Util.TickCount - DataLatency;
            currentInterop.FinishTime = currentInterop.StartTime + 100;
            currentInterop.LastAlpha = 0f;
        }

    }
}
