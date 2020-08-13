using System;
using ProtoBuf;
using Newtonsoft.Json;

namespace Shared.Math
{
    [ProtoContract]
    [ProtoInclude(4, typeof(Quaternion))]
    public class Vector3
    {
        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }

        private static Random Instance = new Random();

        public static Vector3 Zero => new Vector3(0.0f, 0.0f, 0.0f);

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(double x, double y, double z)
        {
            X = (float)x;
            Y = (float)y;
            Z = (float)z;
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            if ((object)left == null && (object)right == null) return true;
            if ((object)left == null || (object)right == null) return false;
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            if ((object)left == null && (object)right == null) return false;
            if ((object)left == null || (object)right == null) return true;
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            if ((object)left == null || (object)right == null) return new Vector3();
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            if ((object)left == null || (object)right == null) return new Vector3();
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3 operator *(Vector3 left, float right)
        {
            if ((object)left == null) return new Vector3();
            return new Vector3(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vector3 operator /(Vector3 left, float right)
        {
            if ((object)left == null) return new Vector3();
            return new Vector3(left.X / right, left.Y / right, left.Z / right);
        }

        public static Vector3 Lerp(Vector3 start, Vector3 end, float n)
        {
            return new Vector3()
            {
                X = start.X + (end.X - start.X) * n,
                Y = start.Y + (end.Y - start.Y) * n,
                Z = start.Z + (end.Z - start.Z) * n,
            };
        }

        public static Vector3 Maximize(Vector3 value1, Vector3 value2)
        {
            Vector3 vector = Zero;
            vector.X = (value1.X > value2.X) ? value1.X : value2.X;
            vector.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            vector.Z = (value1.Z > value2.Z) ? value1.Z : value2.Z;
            return vector;
        }

        public static float Dot(Vector3 left, Vector3 right) => (left.X * right.X + left.Y * right.Y + left.Z * right.Z);

        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            Vector3 result = Zero;
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
            return result;
        }

        public static Vector3 RandomXYZ()
        {
            Vector3 v = Zero;
            double radian = Instance.NextDouble() * 2.0 * System.Math.PI;
            double cosTheta = (Instance.NextDouble() * 2.0) - 1.0;
            double theta = System.Math.Acos(cosTheta);

            v.X = (float)(System.Math.Sin(theta) * System.Math.Cos(radian));
            v.Y = (float)(System.Math.Sin(theta) * System.Math.Sin(radian));
            v.Z = (float)(System.Math.Cos(theta));
            v.Normalize();
            return v;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return a.DistanceTo(b);
        }

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            return a.DistanceToSquared(b);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2}", X, Y, Z);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(LengthSquared());
        }

        public void Normalize()
        {
            var len = Length();

            X = X / len;
            Y = Y / len;
            Z = Z / len;
        }

        [JsonIgnore]
        public Vector3 Normalized
        {
            get
            {
                var len = Length();

                return new Vector3(X / len, Y / len, Z / len);
            }
        }

        public Vector3 Add(Vector3 right)
        {
            return this + right;
        }
        
        public Vector3 Subtract(Vector3 right)
        {
            return this - right;
        }

        public Vector3 Multiply(float right)
        {
            return this*right;
        }

        public Vector3 Divide(float right)
        {
            return this/right;
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            float x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            float y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            float z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            return new Vector3(x, y, z);
        }

        public static Vector3 RandomXY()
        {
            Vector3 v = new Vector3();
            double radian = Instance.NextDouble() * 2 * System.Math.PI;

            v.X = (float)System.Math.Cos(radian);
            v.Y = (float)System.Math.Sin(radian);
            v.Normalize();

            return v;
        }

        public Vector3 Around(float distance)
        {
            return this + RandomXY() * distance;
        }

        public float DistanceToSquared(Vector3 right)
        {
            if ((object)right == null) return 0f;

            var nX = X - right.X;
            var nY = Y - right.Y;
            var nZ = Z - right.Z;

            return nX * nX + nY * nY + nZ * nZ;
        }

        public float DistanceTo(Vector3 right)
        {
            if ((object) right == null) return 0f;
            return (float)System.Math.Sqrt(DistanceToSquared(right));
        }

        public float DistanceToSquared2D(Vector3 right)
        {
            if ((object)right == null) return 0f;

            var nX = X - right.X;
            var nY = Y - right.Y;

            return nX * nX + nY * nY;
        }

        public float DistanceTo2D(Vector3 right)
        {
            if ((object)right == null) return 0f;
            return (float)System.Math.Sqrt(DistanceToSquared2D(right));
        }

        public Vector3()
        { }
    }
}