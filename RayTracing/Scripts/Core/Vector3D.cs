namespace RayTracing
{
    public struct Vector3D
    {
        public static Vector3D Zero => new Vector3D();

        public static Vector3D One => new Vector3D(1, 1, 1);

        public const float EPSILON = 0.00001f;

        public readonly float x;
        public readonly float y;
        public readonly float z;

        public bool IsInfinity
        {
            get
            {
                return x == float.PositiveInfinity || x == float.NegativeInfinity || y == float.PositiveInfinity || y == float.NegativeInfinity || z == float.PositiveInfinity || z == float.NegativeInfinity;
            }
        }

        public bool IsZero
        {
            get
            {
                // Check in epsilon range?
                return x == 0 && y == 0 && z == 0;
            }
        }

        public Vector3D()
        {
            x = 0f;
            y = 0f;
            z = 0f;
        }

        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3D(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(MagnitudeSq());
        }

        public float MagnitudeSq()
        {
            return (x * x) + (y * y) + (z * z);
        }

        public Vector3D Normalize()
        {
            float mag = this.Magnitude();

            return new Vector3D(x / mag, y / mag, z / mag);
        }

        public float DistanceFromSq(Vector3D other)
        {
            return DistanceSq(this, other);
        }

        public float DistanceFrom(Vector3D other)
        {
            return Distance(this, other);
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", x.ToString(), y.ToString(), z.ToString());
        }

        public float this[int dimension]
        {
            get
            {
                if (dimension == 0)
                    return x;
                else if (dimension == 1)
                    return y;
                else if (dimension == 2)
                    return z;

                throw new IndexOutOfRangeException("dimension");
            }
        }

        public static float DistanceSq(Vector3D a, Vector3D b)
        {
            float dSq = (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);

            return dSq;
        }

        public static float Distance(Vector3D a, Vector3D b)
        {
            return MathF.Sqrt(DistanceSq(a, b));
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static bool ArePerpendicular(Vector3D a, Vector3D b)
        {
            return Math.Abs(Vector3D.Dot(a, b)) <= EPSILON;
        }

        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            float _x = a.y * b.z - a.z * b.y;
            float _y = -(a.x * b.z - a.z * b.x);
            float _z = a.x * b.y - a.y * b.x;

            return new Vector3D(_x, _y, _z);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return a * (float)b;
        }

        public static Vector3D operator *(Vector3D a, float b)
        {
            return new Vector3D(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3D operator /(Vector3D a, float b)
        {
            return a * (1f / b);
        }

        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.x, -v.y, -v.z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return a + (-b);
        }
    }
}