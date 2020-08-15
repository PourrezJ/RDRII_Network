using MessagePack;

namespace Shared.Math
{
    [MessagePackObject]
    public class Quaternion : Vector3
    {
        [Key(0)]
        public float W { get; set; }

        public Quaternion()
        { }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion(double x, double y, double z, double w)
        {
            X = (float)x;
            Y = (float)y;
            Z = (float)z;
            W = (float)w;
        }

        public static float Dot(Quaternion left, Quaternion right) => (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);


        public static Quaternion Slerp(Quaternion start, Quaternion end, float amount)
        {
            Quaternion result = new Quaternion();
            float kEpsilon = (float)(1.192093E-07);
            float opposite;
            float inverse;
            float dot = Dot(start, end);

            if (System.Math.Abs(dot) > (1.0f - kEpsilon))
            {
                inverse = 1.0f - amount;
                opposite = amount * System.Math.Sign(dot);
            }
            else
            {
                float acos = (float)System.Math.Acos(System.Math.Abs(dot));
                float invSin = (float)(1.0 / System.Math.Sin(acos));

                inverse = (float)(System.Math.Sin((1.0f - amount) * acos) * invSin);
                opposite = (float)(System.Math.Sin(amount * acos) * invSin * System.Math.Sign(dot));
            }

            result.X = (inverse * start.X) + (opposite * end.X);
            result.Y = (inverse * start.Y) + (opposite * end.Y);
            result.Z = (inverse * start.Z) + (opposite * end.Z);
            result.W = (inverse * start.W) + (opposite * end.W);

            return result;
        }

    }
}
