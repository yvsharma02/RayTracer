namespace RayTracing
{
    public class DefaultShapeShader : ShapeShader
    {
        public readonly Texture MainTexture;

        public readonly Texture NormalMap;

        public DefaultShapeShader(Texture texture = null, Texture normalMap = null)
        {
            this.MainTexture = texture;
            this.NormalMap = normalMap;
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D poc)
        {
            if (NormalMap != null)
            {
                Int2D TextureDimensions = new Int2D(NormalMap.Width - 1, NormalMap.Height - 1);

                Vector2D uv = shape.CalculateUV(poc);

                int x = (int)(uv.x * TextureDimensions.x);
                int y = (int)(uv.y * TextureDimensions.y);

                System.Drawing.Color color = NormalMap[x, y];

                Vector3D nmNormal = new Vector3D((color.R - 127.5f) / 127.5f, (color.G - 127.5f) / 127.5f, color.B / 127.5f);
                Vector3D geomNormal = shape.CalculateNormal(poc).Normalize();

                Vector3D forward = new Vector3D(0, 0, 1);

                Transfomration transform = Transfomration.CalculateRequiredRotationTransform(Vector3D.Zero, forward, geomNormal);

                return transform.Transform(nmNormal).Normalize();
            }
            else
                return shape.CalculateNormal(poc);
        }

        public override Ray[] GetOutgoingRays(Shape shape, Ray tracingRay, Vector3D pointOfContact)
        {
            if (NormalMap != null)
            {
                Int2D TextureDimensions = new Int2D(NormalMap.Width - 1, NormalMap.Height - 1);

                Vector2D uv = shape.CalculateUV(pointOfContact);

                int x = (int)(uv.x * TextureDimensions.x);
                int y = (int)(uv.y * TextureDimensions.y);

                System.Drawing.Color color = MainTexture[x, y];

                return new Ray[] { new Ray(pointOfContact, RTMath.CalculateReflectedRayDirection(tracingRay.DirectionReversed, CalculateNormal(shape, pointOfContact))) };
            }
            else
                return base.GetOutgoingRays(null, tracingRay, pointOfContact);
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
                        float dot = Vector3D.Dot(hittingRays[i][j].Direction * -1f, CalculateNormal(shape, pointOfContact));

                        if (dot < 0f)
                            dot = 0f;

                        ColoredRay ray = hittingRays[i][j];
                        _i += ray.DestinationColor.Intensity;
                        r += (float)(ray.SourceColor.R * dot * (ray.DestinationColor.Intensity / totalIntensity));
                        g += (float)(ray.SourceColor.G * dot * (ray.DestinationColor.Intensity / totalIntensity));
                        b += (float)(ray.SourceColor.B * dot * (ray.DestinationColor.Intensity / totalIntensity));

                    }
                }
            }
            else
                return RTColor.Black;

            if (MainTexture != null)
            {
                Int2D TextureDimensions = new Int2D(MainTexture.Width - 1, MainTexture.Height - 1);

                Vector2D uv = shape.CalculateUV(pointOfContact);

                int x = (int)(uv.x * TextureDimensions.x);
                int y = (int)(uv.y * TextureDimensions.y);

                System.Drawing.Color color = MainTexture[x, y];
                
                r = (r + color.R) / 2f;
                g = (g + color.G) / 2f;
                b = (b + color.B) / 2f;
            }

            return new RTColor(_i, r, g, b);
        }
    }
}