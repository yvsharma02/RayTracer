namespace RayTracing
{
    public struct RTColor
    {
        public static readonly RTColor Black = new RTColor(0, 0, 0, 0);

        private readonly int irgb;

        public int Intensity
        {
            get
            {
                return irgb >> 24 & (0x00FF);
            }
        }

        public int R
        {
            get
            {
                return irgb >> 16 & (0x00FF);
            }
        }

        public int G
        {
            get
            {
                return irgb >> 8 & (0x00FF);
            }
        }

        public int B
        {
            get
            {
                return irgb & (0x00FF);
            }
        }

        /*
        public static RTColor Add(RTColor clrA, RTColor clrB, Ray rayA, Ray rayB, Ray rayOut, float dropOffStrength)
        {
            throw new NotImplementedException();
        }
        */

        public static RTColor Add(RTColor clrA, RTColor clrB)
        {
            float i = clrA.Intensity + clrB.Intensity;
            float r = 2 * (clrA.R * clrA.Intensity + clrB.R * clrB.Intensity) / i;
            float g = 2 * (clrA.B * clrA.Intensity + clrB.B * clrB.Intensity) / i;
            float b = 2 * (clrA.B * clrA.Intensity + clrB.B * clrB.Intensity) / i;

            return new RTColor(i, r, g, b);
        }

        public System.Drawing.Color ToARGB()
        {
            float r = (Intensity / 255f) * R;
            float g = (Intensity / 255f) * G;
            float b = (Intensity / 255f) * B;

            return System.Drawing.Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        public static RTColor Multiply(RTColor clr, float multiplier)
        {
            return new RTColor(clr.Intensity * multiplier, clr.R, clr.G, clr.B);
        }

        public RTColor(int intensity, int r, int g, int b)
        {
            irgb = (intensity << 24) | (r << 16) | (g << 8) | b;
        }

        public RTColor(float intensity, float r, float g, float b) : this((int) intensity, (int) r, (int) g, (int) b) {}

        public static RTColor operator *(RTColor a, float multiplier)
        {
            return RTColor.Multiply(a, multiplier);
        }

        public override string ToString()
        {
            return String.Format("(I:{0}, R:{1}, G:{2}, B:{3})", Intensity, R, G, B);
        }

    }
}
