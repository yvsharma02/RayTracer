namespace RayTracing
{
    // Because RTColor is bounded, and sometimes color exceeds the bonud during calculations.
    public struct RawRTColor
    {
        public readonly float Intensity;

        public readonly float R;
        public readonly float G;
        public readonly float B;

        public readonly float AbsoluteR => R * RTColor.COLOR_CHANNEL_SCALE;
        public readonly float AbsoluteG => G * RTColor.COLOR_CHANNEL_SCALE;
        public readonly float AbsoluteB => B * RTColor.COLOR_CHANNEL_SCALE;

        public RawRTColor(float intensity, float r, float g, float b)
        {
            this.Intensity = intensity;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public static implicit operator RawRTColor(RTColor clr) => new RawRTColor(clr.Intensity, clr.R, clr.G, clr.B);

        public static explicit operator RTColor(RawRTColor clr) => new RTColor(clr.Intensity, clr.R, clr.G, clr.B);

        public static RawRTColor operator +(RawRTColor a, RawRTColor b) => new RawRTColor(a.Intensity + b.Intensity, a.R + b.R, a.G + b.G, a.B + b.B);

        public static RawRTColor operator -(RawRTColor clr) => new RawRTColor(-clr.Intensity, -clr.R, -clr.G, -clr.B);

        public static RawRTColor operator -(RawRTColor a, RawRTColor b) => a + (-b);

        public static RawRTColor operator *(RawRTColor clr, float b) => new RawRTColor(clr.Intensity, clr.R * b, clr.G * b, clr.B * b);

        public static RawRTColor operator /(RawRTColor clr, float b) => clr * (1f / b);
    }
}