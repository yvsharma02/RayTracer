namespace RayTracing
{
    public struct Quaternion
    {
        public static Quaternion Identity => new Quaternion(1f, 0f, 0f, 0f);

        public readonly float real;

        public readonly float i;
        public readonly float j;
        public readonly float k;

        public Vector3D ImaginaryAxis => new Vector3D(i, j, k);

        public Quaternion Conjugate => new Quaternion(real, -i, -j, -k);

        public Quaternion Inverse => Conjugate / MagnitudeSq;

        public float MagnitudeSq => real * real + i * i + j * j + k * k;

        public float Magnitude => MathF.Sqrt(MagnitudeSq);

        public Quaternion(float r, float i, float j, float k)
        {
            float mag = 1;// MathF.Sqrt(r * r + i * i + j * j + k * k);

            this.real = r / mag;
            this.i = i / mag;
            this.j = j / mag;
            this.k = k / mag;
        }

        public Quaternion(float real, Vector3D img) : this(real, img.x, img.y, img.z) { }

        public override string ToString()
        {
            return String.Format("{0} + {1}i + {2}j + {3}k :: Euler angles: {4}", real, i, j, k, ToEulerAngles() * RTMath.RAD_TO_DEG);
        }

        public Vector3D Rotate(Vector3D point)
        {
            Quaternion result = this * point * this.Inverse;

            return result.ImaginaryAxis;
        }

        public Vector3D ToEulerAngles()
        {
            Quaternion q = this;

            float eax = 0f;
            float eay = 0f;
            float eaz = 0f;

            float sinRcoP = (q.real * q.i + q.j * q.k) * 2f;
            float cosRcosP = 1f - (q.i * q.i + q.j * q.j) * 2f;
            eax = MathF.Atan2(sinRcoP, cosRcosP);

            float sinP = 2f * (q.real * q.j - q.k * q.i);
            
            if (MathF.Abs(sinP) >= 1)
                eay = MathF.CopySign(MathF.PI / 2f, sinP);
            else
                eay = MathF.Sin(sinP);

            float sinYcosP = (q.real * q.k + q.i * q.j) * 2f;
            float cosYcosP = 1f - (q.j * q.j + q.k * q.k) * 2f;
            eaz = MathF.Atan2(sinYcosP, cosYcosP);

            return new Vector3D(eax, eay, eaz);
        }

        public static Quaternion FromEulerAngles(Vector3D eulerAngles)
        {
            float cz = MathF.Cos(eulerAngles.z * 0.5f);
            float sz = MathF.Sin(eulerAngles.z * 0.5f);

            float cy = MathF.Cos(eulerAngles.y * 0.5f);
            float sy = MathF.Sin(eulerAngles.y * 0.5f);

            float cx = MathF.Cos(eulerAngles.x * 0.5f);
            float sx = MathF.Sin(eulerAngles.x * 0.5f);

            float _r = cx * cy * cz + sx * sy * sz;
            float _i = sx * cy * cz - cx * sy * sz;
            float _j = cx * sy * cz + sx * cy * sz;
            float _k = cx * cy * sz - sx * sy * cz;
        
            return new Quaternion(_r, _i, _j, _k);
        }

        public static Quaternion CreateRotationQuaternion(Vector3D axis, float angleRad)
        {
            float sin = MathF.Sin(angleRad / 2f);
            float cos = MathF.Cos(angleRad / 2f);

            return new Quaternion(cos, axis.Normalize() * sin);
        }

        public static Quaternion Multipliply(Quaternion q1, Quaternion q2)
        {
            float _r = q1.real * q2.real - q1.i * q2.i - q1.j * q2.j - q1.k * q2.k;
            float _i = q1.real * q2.i + q1.i * q2.real + q1.j * q2.k - q1.k * q2.j;
            float _j = q1.real * q2.j - q1.i * q2.k + q1.j * q2.real + q1.k * q2.i;
            float _k = q1.real * q2.k + q1.i * q2.j - q1.j * q2.i + q1.k * q2.real;

            return new Quaternion(_r, _i, _j, _k);
        }

        public static Quaternion operator *(Quaternion a, Vector3D b) => Multipliply(a, (Quaternion)b);

        public static Quaternion operator *(Quaternion a, Quaternion b) => Multipliply(a, b);

        public static Quaternion operator *(Quaternion q, float scalar) => new Quaternion(q.real * scalar, q.i * scalar, q.j * scalar, q.k * scalar);

        public static Quaternion operator /(Quaternion q, float scalar) => q * (1f / scalar);

        public static explicit operator Quaternion(Vector3D v) => new Quaternion(0f, v.x, v.y, v.z);

        public static explicit operator Vector3D(Quaternion q) => new Vector3D(q.i, q.j, q.k);
    }
}