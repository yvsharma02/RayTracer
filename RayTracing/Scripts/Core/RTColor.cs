using System.Diagnostics.CodeAnalysis;

namespace RayTracing
{
    public struct RTColor
    {
        public const double MAX_INTENSITY = 100;

        public static readonly RTColor Black = new RTColor(0, 0, 0, 0);

        private readonly double intensity;
        private readonly int rgb;

        public double Intensity
        {
            get
            {
                return intensity;
            }
        }

        public int R
        {
            get
            {
                return rgb >> 16 & (0x00FF);
            }
        }

        public int G
        {
            get
            {
                return rgb >> 8 & (0x00FF);
            }
        }

        public int B
        {
            get
            {
                return rgb & (0x00FF);
            }
        }

        // Not trying to be accurate, just trying to look good.
        public static RTColor CalculateDropOffColor(RTColor color, Vector3D a, Vector3D b)
        {
            if (!a.IsInfinity && !b.IsInfinity)
            {
                double distSq = Vector3D.DistanceSq(a, b);

                if (distSq > 1)
                    distSq = Math.Sqrt(distSq);

                distSq /= MAX_INTENSITY;

                return new RTColor(color.Intensity / distSq, color.R, color.G, color.B);
            }
            return color;
        }
        public System.Drawing.Color ToARGB()
        {
            float r = (float) (Intensity / MAX_INTENSITY) * R;
            float g = (float) (Intensity / MAX_INTENSITY) * G;
            float b = (float) (Intensity / MAX_INTENSITY) * B;

            return System.Drawing.Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        public static RTColor Multiply(RTColor clr, float multiplier)
        {
            return new RTColor(clr.Intensity * multiplier, clr.R, clr.G, clr.B);
        }

        public RTColor(double intensity, int r, int g, int b)
        {
            if (intensity > MAX_INTENSITY)
                intensity = MAX_INTENSITY;

            this.intensity = intensity;
            rgb = (r << 16) | (g << 8) | b;
        }

        public RTColor(double intensity, float r, float g, float b) : this(intensity, (int) r, (int) g, (int) b) {}

        public static RTColor operator *(RTColor a, float multiplier)
        {
            return RTColor.Multiply(a, multiplier);
        }

        public override string ToString()
        {
            return String.Format("(I:{0}, R:{1}, G:{2}, B:{3})", Intensity, R, G, B);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if(!(obj is RTColor))
                return false;
            RTColor other = (RTColor) obj;

            return other.rgb == rgb && other.Intensity == Intensity;
        }

    }
}
