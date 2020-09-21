using ResuMPServer.Constant;
using Shared;
using Shared.Math;

namespace ResuMPServer
{
    public class Marker : Entity
    {
        internal Marker(API father, NetHandle handle) : base(father, handle)
        {
        }

        #region Properties

        public int MarkerType
        {
            get => GetMarkerType();
            set => SetMarkerType(value);
        }

        public Vector3 Scale
        {
            get => GetMarkerScale();
            set => SetMarkerScale(value);
        }

        public Vector3 Direction
        {
            get => GetMarkerDirection();
            set => SetMarkerDirection(value);
        }

        public Color Color
        {
            get => GetMarkerColor();
            set => SetMarkerColor(value.alpha, value.red, value.green, value.blue);
        }

        #endregion

        #region Methods
        public void SetMarkerType(int type)
        {
            if (DoesEntityExist())
            {
                ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).MarkerType = type;

                var delta = new MarkerProperties();
                delta.MarkerType = type;
                GameServer.UpdateEntityInfo(Value, EntityType.Marker, delta);
            }
        }

        public int GetMarkerType()
        {
            if (DoesEntityExist())
            {
                return ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).MarkerType;
            }

            return 0;
        }

        public void SetMarkerScale(Vector3 scale)
        {
            if (DoesEntityExist())
            {
                ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Scale = scale;

                var delta = new MarkerProperties();
                delta.Scale = scale;
                GameServer.UpdateEntityInfo(Value, EntityType.Marker, delta);
            }
        }

        public Vector3 GetMarkerScale()
        {
            if (DoesEntityExist())
            {
                return ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Scale;
            }

            return null;
        }

        public void SetMarkerDirection(Vector3 dir)
        {
            if (DoesEntityExist())
            {
                ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Direction = dir;

                var delta = new MarkerProperties();
                delta.Direction = dir;
                GameServer.UpdateEntityInfo(Value, EntityType.Marker, delta);
            }
        }


        public Vector3 GetMarkerDirection()
        {
            if (DoesEntityExist())
            {
                return ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Direction;
            }

            return null;
        }

        public void SetMarkerColor(int alpha, int red, int green, int blue)
        {
            if (DoesEntityExist())
            {
                var entity = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]);

                entity.Alpha = (byte)alpha;
                entity.Red = (byte)red;
                entity.Green = (byte)green;
                entity.Blue = (byte)blue;

                var delta = new MarkerProperties();
                delta.Alpha = (byte)alpha;
                delta.Red = (byte)red;
                delta.Green = (byte)green;
                delta.Blue = (byte)blue;
                GameServer.UpdateEntityInfo(Value, EntityType.Marker, delta);
            }
        }

        public Color GetMarkerColor()
        {
            Color output = new Color();

            if (DoesEntityExist())
            {
                var entity = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]);
                output = new Color(entity.Alpha, entity.Red, entity.Green, entity.Blue);
            }

            return output;
        }
        #endregion
    }
}