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

        /*
        public static RTColor Add(RTColor clrA, RTColor clrB, Ray rayA, Ray rayB, Ray rayOut, float dropOffStrength)
        {
            throw new NotImplementedException();
        }
        */

        public static RTColor Average(params RTColor[] clrs)
        {
            double _i = 0f;
            float r = 0f, g = 0f, b = 0f;

            for (int i = 0; i < clrs.Length; i++)
            {
                _i += clrs[i].Intensity / ((double) clrs.Length);
                r += clrs[i].R / ((float)clrs.Length);
                g += clrs[i].G / ((float)clrs.Length);
                b += clrs[i].B / ((float)clrs.Length);
            }

            return new RTColor(_i, r, g, b);
        }

        public static RTColor Average(RTColor[][] clrs)
        {
            int count = 0;
            for (int i = 0; i < clrs.Length; i++)
                count += clrs[i].Length;

            double _i = 0f;
            float r = 0f, g = 0f, b = 0f;

            for (int i = 0; i < clrs.GetLength(0); i++)
            {
                for (int j = 0; j < clrs[i].Length; j++)
                {
                    _i += clrs[i][j].Intensity / ((double) count);
                    r += clrs[i][j].R / ((float)count);
                    g += clrs[i][j].G / ((float)count);
                    b += clrs[i][j].B / ((float)count);
                }
            }

            return new RTColor(_i, r, g, b);
        }

        public static RTColor Add(RTColor clrA, RTColor clrB)
        {
            double i = clrA.Intensity + clrB.Intensity;
            float r = (float) (2 * (clrA.R * clrA.Intensity + clrB.R * clrB.Intensity) / i);
            float g = (float) (2 * (clrA.B * clrA.Intensity + clrB.B * clrB.Intensity) / i);
            float b = (float) (2 * (clrA.B * clrA.Intensity + clrB.B * clrB.Intensity) / i);

            return new RTColor(i, r, g, b);
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

        public RTColor(double intensity, float r, float g, float b) : this((int) intensity, (int) r, (int) g, (int) b) {}

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
