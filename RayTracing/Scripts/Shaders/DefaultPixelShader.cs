using System.Drawing;

namespace RayTracing
{
    public class DefaultPixelShader : PixelShader
    {
        public readonly Int2D RaysPerPixel;

        public DefaultPixelShader(Int2D raysPerPixel)
        {
            this.RaysPerPixel = raysPerPixel;
        }

        public override Ray[] GetEmmitedRays(Camera camera, Int2D pixel)
        {
            int c = 0;
            Ray[] rays = new Ray[RaysPerPixel.x * RaysPerPixel.y];

            Vector3D origin = camera.EyePosition;

            for (int i = 0; i < RaysPerPixel.x; i++)
            {
                for (int j = 0; j < RaysPerPixel.y; j++)
                {
                    float percentX = (pixel.x * RaysPerPixel.x + i) / (float)((camera.Resolution.x + 1) * RaysPerPixel.x);
                    float percentY = (pixel.y * RaysPerPixel.y + j) / (float)((camera.Resolution.y + 1) * RaysPerPixel.y);

                    Vector3D screenPt = camera.ProjectedTopLeft + (camera.ProjectedTopRight - camera.ProjectedTopLeft) * percentX - (camera.ProjectedTopLeft - camera.ProjectedButtomLeft) * percentY;
                    Vector3D dir = screenPt - origin;

                    rays[c++] = new Ray(origin, dir);
                }
            }

            return rays;
        }

        public override Color CalculateFinalPixelColor(Camera camera, Int2D pixelIndex, ColoredRay[] hittinRays)
        {
            double totalIntensity = 0;

            for (int i = 0; i < hittinRays.Length; i++)
                totalIntensity += hittinRays[i].SourceColor.Intensity;

            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < hittinRays.Length; i++)
            {
                float multiplier = (float)((hittinRays[i].SourceColor.Intensity / RTColor.MAX_INTENSITY) / hittinRays.Length);

                r += hittinRays[i].SourceColor.R * multiplier;
                g += hittinRays[i].SourceColor.G * multiplier;
                b += hittinRays[i].SourceColor.B * multiplier;
            }
             
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            if (!float.IsNormal(r) || r < 0f)
                r = 0f;
            if (!float.IsNormal(g) || g < 0f)
                g = 0f;
            if (!float.IsNormal(b) || b < 0f)
                b = 0f;

            return Color.FromArgb(255, (byte) r, (byte) g, (byte) b);
        }
    }
}