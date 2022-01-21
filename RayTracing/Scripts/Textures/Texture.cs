using System.Drawing;
#pragma warning disable CA1416

namespace RayTracing
{
    public class Texture
    {
        private readonly Color[,] pixels;
        public readonly string Location;

        public int Height
        {
            get
            {
                return pixels.GetLength(1);
            }
        }

        public int Width
        {
            get
            {
                return pixels.GetLength(0);
            }
        }

        public Texture(String location)
        {
            this.Location = location;
            Bitmap bmp = new Bitmap(location);

            pixels = new Color[bmp.Width, bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                    pixels[i, j] = bmp.GetPixel(i, j);
        }

        public Color this[int x, int y]
        {
            get
            {
                return pixels[x, y];
            }
        }

        public Color this[Int2D index]
        {
            get
            {
                return pixels[index.x, index.y];
            }
        }

        public Color GetColorFromUV(float x, float y)
        {
            if (x < 0 || y < 0 || x > 1f || y > 1f)
                throw new ArgumentException("x and y should be between 0 and 1f.");

            int finalX = (int)(x * (Width - 1));
            int finalY = (int)(y * (Height - 1));

            return pixels[finalX, finalY];
        }

        public Color GetColorFromUV(Vector2D uv)
        {
            return GetColorFromUV(uv.x, uv.y);
        }
    }
}