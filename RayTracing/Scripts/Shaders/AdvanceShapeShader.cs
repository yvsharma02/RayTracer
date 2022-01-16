﻿namespace RayTracing
{
    public class AdvanceShapeShader : ShapeShader
    {
        public readonly Texture MainTexture;
        public readonly Texture NormalMap;

        public readonly float Absorbance = 0f;
        public readonly float LightingStrength = 1f;

        public AdvanceShapeShader(Texture mainTexture, Texture normalMap, float absorbance, float lightingStrength)
        {
            this.MainTexture = mainTexture;
            this.NormalMap = normalMap;
            this.Absorbance = absorbance;
            this.LightingStrength = lightingStrength;
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

        public override RTColor CalculateBounceColor(Shape shape, ColoredRay[][] hittingRays, Vector3D pointOfContact, Vector3D outgoingRayDir)
        {
            double totalIntensity = 0f;

            for (int i = 0; i < hittingRays.Length; i++)
                for (int j = 0; j < hittingRays[i].Length; j++)
                    totalIntensity += hittingRays[i][j].DestinationColor.Intensity;

            float r = 0f;
            float g = 0f;
            float b = 0f;

            for (int i = 0; i < hittingRays.Length; i++)
            {
                for (int j = 0; j < hittingRays[i].Length; j++)
                {
                    float dot = 1f;

                    if (!hittingRays[i][j].Direction.IsZero)
                        dot = Vector3D.Dot(hittingRays[i][j].Direction * -1f, CalculateNormal(shape, pointOfContact));

                    if (dot < 0f)
                        dot = 0f;

                    RTColor clr = hittingRays[i][j].SourceColor;

                    r += clr.R * dot;
                    g += clr.G * dot;
                    b += clr.B * dot;

                }
            }

            if (MainTexture != null)
            {
                var textureClr = MainTexture.GetColorFromUV(shape.CalculateUV(pointOfContact));
                r = (textureClr.R + LightingStrength * r) / (1f + LightingStrength);
                g = (textureClr.G + LightingStrength * g) / (1f + LightingStrength);
                b = (textureClr.B + LightingStrength * b) / (1f + LightingStrength);
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