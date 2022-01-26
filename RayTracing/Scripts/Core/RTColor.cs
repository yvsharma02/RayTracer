using System.Diagnostics.CodeAnalysis;

namespace RayTracing
{
    public struct RTColor
    {
        public const float COLOR_CHANNEL_SCALE = 500;
        public const float MAX_INTENSITY = 500;

        public static readonly RTColor Black = new RTColor(0, 0, 0, 0);

        private readonly float intensity;
        private readonly float r;
        private readonly float g;
        private readonly float b;

        public float R => r;
        public float G => g;
        public float B => b;

        public float Intensity => intensity;

        public float AbsoluteR => r * COLOR_CHANNEL_SCALE;
        public float AbsoluteG => g * COLOR_CHANNEL_SCALE;
        public float AbsoluteB => b * COLOR_CHANNEL_SCALE;

        public RTColor(float intensity, float r, float g, float b)
        {
            if (intensity > MAX_INTENSITY)
                intensity = MAX_INTENSITY;

            if (intensity < 0)
                intensity = 0f;

            if (r > 1f)
                r = 1f;
            if (g > 1f)
                g = 1f;
            if (b > 1f)
                b = 1f;

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            this.intensity = intensity;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public override string ToString()
        {
            return String.Format("(I:{0}, R:{1}, G:{2}, B:{3})", Intensity, AbsoluteR, AbsoluteG, AbsoluteB);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is RTColor))
                return false;
            RTColor other = (RTColor)obj;

            return other.r == r && other.g == g && other.b == b && other.intensity == intensity;
        }

        // Not trying to be accurate, just trying to look good.
        public static RTColor CalculateDropOffColor(RTColor color, Vector3D a, Vector3D b)
        {
            /*
            if (!a.IsInfinity && !b.IsInfinity)
            {
                float distSq = Vector3D.DistanceSq(a, b);

                if (distSq > 1)
                    distSq = MathF.Sqrt(distSq);

                distSq /= MAX_INTENSITY;

                return new RTColor(color.Intensity / distSq, color.AbsoluteR, color.AbsoluteG, color.AbsoluteB);
            }
            */
            return color;
        }

        /*
        public static RTColor operator *(RTColor clr, float multiplier) => new RTColor(clr.Intensity, clr.r * multiplier, clr.g * multiplier, clr.b * multiplier);

        public static RTColor operator *(float multiplier, RTColor clr) => clr * multiplier;

        public static RTColor operator /(RTColor a, float b) => a * (1f / b);

        public static RTColor operator +(RTColor a, RTColor b) => new RTColor(a.intensity + b.intensity, a.r + b.r, a.g + b.g, a.b + b.b);

        public static RTColor operator -(RTColor a, RTColor b) => new RTColor(a.Intensity - b.Intensity, a.r - b.r, a.g - b.g, a.b - b.b);
        */
    }
}