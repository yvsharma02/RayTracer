namespace RayTracing
{
    public abstract class PixelShader
    {
        public abstract Ray[] GetEmmitedRays(Camera camera, Int2D pixelIndex);

        public abstract System.Drawing.Color CalculateFinalPixelColor(Camera camera, Int2D pixelIndex, EmmisionChain[] hittingRays);

    }
}