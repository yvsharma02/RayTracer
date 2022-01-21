namespace RayTracing
{
    public class AdvanceShapeShader : ShapeShader
    {
        public readonly Texture MainTexture;
        public readonly Texture NormalMap;

        public readonly float Reflectiveness = 0f;
        public readonly float Absorbance = 0f;
        public readonly float TextureStrength = 1f;

        public AdvanceShapeShader(Texture mainTexture, Texture normalMap, float absorbance, float textureStrength, float reflectiveness)
        {
            this.Reflectiveness = reflectiveness;
            this.MainTexture = mainTexture;
            this.NormalMap = normalMap;
            this.Absorbance = absorbance;
            this.TextureStrength = textureStrength;
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D poc)
        {
            if (NormalMap == null)
                return shape.CalculateNormal(poc);

            System.Drawing.Color normalColor = NormalMap.GetColorFromUV(shape.CalculateUV(poc));

            Vector3D nmNormal = new Vector3D((normalColor.R - 127.5f) / 127.5f, (normalColor.G - 127.5f) / 127.5f, normalColor.B / 127.5f);
            Vector3D geomNormal = shape.CalculateNormal(poc).Normalize();

            Vector3D forward = new Vector3D(0, 0, 1);

            Transfomration transform = Transfomration.CalculateRequiredRotationTransform(Vector3D.Zero, forward, geomNormal);

            return transform.Transform(nmNormal).Normalize();
        }

        public override RTColor CalculateBounceColor(Shape shape, EmmisionChain[] hittingRays, Vector3D pointOfContact, Vector3D outgoingRayDir)
        {
            double totalIntensity = 0f;
            double maxIntensity = 0f;

            for (int i = 0; i < hittingRays.Length; i++)
            {
                if (maxIntensity < hittingRays[i].EmmitedRay.DestinationColor.Intensity)
                    maxIntensity = hittingRays[i].EmmitedRay.DestinationColor.Intensity;

                totalIntensity += hittingRays[i].EmmitedRay.DestinationColor.Intensity;
            }

            float r = 0f;
            float g = 0f;
            float b = 0f;

            for (int i = 0; i < hittingRays.Length; i++)
            {
                float dot = 1f;
                float weight = 1f;
                float reflectionMultiplier = 1;

                if ((hittingRays[i].Emmiter.TypeID & (int) TypeIDs.Shape) != 0)
                    reflectionMultiplier = Reflectiveness;

                if (!hittingRays[i].EmmitedRay.Direction.IsZero)
                    dot = Vector3D.Dot(hittingRays[i].EmmitedRay.Direction * -1f, CalculateNormal(shape, pointOfContact));
                else if (hittingRays[i].EmmitedRay.PointOfContact.IsInfinity)
                    dot = 1f;

                if (dot < 0f)
                    dot = 0f;

                weight = (float)(hittingRays[i].EmmitedRay.DestinationColor.Intensity / maxIntensity);

                RTColor clr = hittingRays[i].EmmitedRay.DestinationColor;

                r += clr.R * dot * weight * reflectionMultiplier;
                g += clr.G * dot * weight * reflectionMultiplier;
                b += clr.B * dot * weight * reflectionMultiplier;
            }

            if (MainTexture != null)
            {
                System.Drawing.Color textureClr = MainTexture.GetColorFromUV(shape.CalculateUV(pointOfContact));

                r = (textureClr.R * TextureStrength + r) / (1f + TextureStrength);
                g = (textureClr.G * TextureStrength + g) / (1f + TextureStrength);
                b = (textureClr.B * TextureStrength + b) / (1f + TextureStrength);
            }

            return new RTColor(totalIntensity * (1 - Absorbance), r, g, b);
        }

        public override Ray[] GetOutgoingRays(Shape shape, Ray tracingRay, Vector3D pointOfContact)
        {
            if (NormalMap != null)
                return new Ray[] { new Ray(pointOfContact, RTMath.CalculateReflectedRayDirection(tracingRay.DirectionReversed, CalculateNormal(shape, pointOfContact))) };
            else
                return base.GetOutgoingRays(shape, tracingRay, pointOfContact);
        }
    }
}