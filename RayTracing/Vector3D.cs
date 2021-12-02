namespace RayTracing
{
    public struct Vector3D
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

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

        public float Magnitute()
        {
            return (float)Math.Sqrt(MagnitudeSq());
        }

        public float MagnitudeSq()
        {
            return (x * x) + (y * y) + (z * z);
        }

        public Vector3D Normalize()
        {
            float mag = this.Magnitute();

            return new Vector3D(x / mag, y / mag, z / mag);
        }

        public float DistanceFrom(Vector3D other)
        {
            return Distance(this, other);
        }

        public static float Distance(Vector3D a, Vector3D b)
        {
            return (float) Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
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