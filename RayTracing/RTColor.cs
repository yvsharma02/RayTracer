namespace RayTracing
{
    public struct RTColor
    {
        private readonly int channels;

        public int Intensity
        {
            get
            {
                return channels >> 24;
            }
        }

        public int R
        {
            get
            {
                return channels >> 16 & (0x000F);
            }
        }

        public int G
        {
            get
            {
                return channels >> 8 & (0x000F);
            }
        }

        public int B
        {
            get
            {
                return channels & (0x000F);
            }
        }

        public RTColor(int intensity, int r, int g, int b)
        {
            channels = intensity << 24 | r << 16 | g << 8 | b;
        }

        public RTColor(float intensity, float r, float g, float b) : this((int) intensity, (int) r, (int) g, (int) b) {}

    }
}
