namespace RayTracing
{
    public class DefaultShapeShader : ShapeShader
    {
        public readonly Texture MainTexture;

        public DefaultShapeShader() { }

        public DefaultShapeShader(Texture texture)
        {
            this.MainTexture = texture;
        }

        public override RTColor CalculateBounceColor(Shape shape, ColoredRay[][] hittingRays, Vector3D pointOfContact, Vector3D bouncedRayDirection)
        {
            int count = 0;

            double totalIntensity = 0f;

            for (int i = 0; i < hittingRays.Length; i++)
            {
                count += hittingRays[i].Length;
                for (int j = 0; j < hittingRays[i].Length; j++)
                    totalIntensity += hittingRays[i][j].DestinationColor.Intensity;
            }

            double _i = 0f;
            float r = 0f, g = 0f, b = 0f;

            if (totalIntensity > Vector3D.EPSILON)
            {
                for (int i = 0; i < hittingRays.GetLength(0); i++)
                {
                    for (int j = 0; j < hittingRays[i].Length; j++)
                    {
                        ColoredRay ray = hittingRays[i][j];

                        _i += ray.DestinationColor.Intensity;
                        r += (float)(ray.SourceColor.R * (ray.DestinationColor.Intensity / totalIntensity));
                        g += (float)(ray.SourceColor.G * (ray.DestinationColor.Intensity / totalIntensity));
                        b += (float)(ray.SourceColor.B * (ray.DestinationColor.Intensity / totalIntensity));

                    }
                }
            }
            else
                return RTColor.Black;

            if (MainTexture != null)
            {
                Int2D TextureDimensions = new Int2D(MainTexture.Width - 1, MainTexture.Height - 1);

                r = (r + MainTexture[shape.POCToTexturePixelIndex(null, pointOfContact, TextureDimensions)].R) / 2f;
                g = (g + MainTexture[shape.POCToTexturePixelIndex(null, pointOfContact, TextureDimensions)].G) / 2f;
                r = (b + MainTexture[shape.POCToTexturePixelIndex(null, pointOfContact, TextureDimensions)].B) / 2f;
            }

            return new RTColor(_i, r, g, b);
        }
    }
}