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

        public  float Magnitute()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public Vector3D Normalize()
        {
            float mag = this.Magnitute();

            return new Vector3D(x / mag, y / mag, z / mag);
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.y * b.z - a.z * b.y, -(a.x * b.z - a.z * b.x), a.x * b.y - b.y * a.x);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
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