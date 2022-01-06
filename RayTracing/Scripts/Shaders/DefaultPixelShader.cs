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
                    //                    Vector3D subPixelPosition = pixelTopLeft + new Vector3D(pixelSize.x * ((float) (i + 1) / (RaysPerPixel.x + 1)), ((float) pixelSize.y * (j + 1) / ( RaysPerPixel.y + 1)));

                    //                    rays[c++] = new Ray(rayOrigin, subPixelPosition - rayOrigin);



                    float percentX = (pixel.x * RaysPerPixel.x + i) / (float)((camera.Resolution.x + 1) * RaysPerPixel.x);
                    float percentY = (pixel.y * RaysPerPixel.y + j) / (float)((camera.Resolution.y + 1) * RaysPerPixel.y);

                    Vector3D screenPt = camera.ProjectedTopLeft + (camera.ProjectedTopRight - camera.ProjectedTopLeft) * percentX - (camera.ProjectedTopLeft - camera.ProjectedButtomLeft) * percentY;
                    Vector3D dir = screenPt - origin;

                    rays[c++] = new Ray(origin, dir);
                }
            }

            return rays;
        }

        public override Color CalculateFinalPixelColor(Camera camera, RTColor[] hittingRayColors)
        {
            double totalIntensity = 0;

            for (int i = 0; i < hittingRayColors.Length; i++)
                totalIntensity += hittingRayColors[i].Intensity;

            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < hittingRayColors.Length; i++)
            {
                float multiplier = (float) (hittingRayColors[i].Intensity / totalIntensity);

                r += hittingRayColors[i].R * multiplier;
                g += hittingRayColors[i].G * multiplier;
                b += hittingRayColors[i].B * multiplier;
            }

            return Color.FromArgb(255, (byte) r, (byte) g, (byte) b);
        }
    }
}