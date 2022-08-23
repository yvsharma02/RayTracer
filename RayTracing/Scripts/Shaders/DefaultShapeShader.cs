namespace RayTracing
{
    public class DefaultShapeShader : ShapeShader
    {
        public readonly Texture MainTexture;
        public readonly Texture NormalMap;

        public readonly float Reflectiveness = 0f;
        public readonly float Absorbance = 0f;
        public readonly float TextureStrength = 1f;

        public DefaultShapeShader(Texture mainTexture, Texture normalMap, float absorbance = 0f, float textureStrength = 1f, float reflectiveness = 0f)
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
            
            MeshTriangle triangle = (MeshTriangle)shape;

            Vector2D uv = shape.CalculateUV(poc);
            System.Drawing.Color normalColor = NormalMap.GetColorFromUV(uv);

            Vector3D nmNormal = new Vector3D((normalColor.R / 127.5f) - 1f, (normalColor.G / 127.5f) - 1f, (normalColor.B / 255f));
            Vector3D geomNormal = shape.CalculateNormal(poc).Normalize();

            Vector3D forward = new Vector3D(0, 0, 1);

            Vector3D cross = Vector3D.Cross(forward, geomNormal);
            float r = MathF.Sqrt(1 + Vector3D.Dot(forward, geomNormal));

            Quaternion rotationQ = new Quaternion(r, cross);

            Vector3D rotatedNormal = rotationQ.Rotate(nmNormal);

            return rotatedNormal;
        }
        
        public override RTColor CalculateBounceColor(Shape shape, EmmisionChain[] hittingRays, Vector3D pointOfContact, Vector3D outgoingRayDir)
        {
            float lightOnlyIntensity = 0f;
            float totalIntensity = 0f;

            float maxIntensity = float.NegativeInfinity;

            RawRTColor totalColor = RTColor.Black;
            RawRTColor lightOnlyColor = RTColor.Black;

            for (int i = 0; i < hittingRays.Length; i++)
            {
                totalIntensity += hittingRays[i].EmmitedRay.DestinationColor.Intensity;
 
                if (hittingRays[i].LastEmmiter != null && ((hittingRays[i].LastEmmiter.TypeID & (int)TypeID.Light) != 0))
                    lightOnlyIntensity += hittingRays[i].EmmitedRay.DestinationColor.Intensity;

                if (maxIntensity < hittingRays[i].EmmitedRay.DestinationColor.Intensity)
                    maxIntensity = hittingRays[i].EmmitedRay.DestinationColor.Intensity;
            }

            for (int i = 0; i < hittingRays.Length; i++)
            {
                bool comingFromLightSource = hittingRays[i].LastEmmiter != null && ((hittingRays[i].LastEmmiter.TypeID & (int)TypeID.Light) != 0);

                float lightOnlyWeight = hittingRays[i].EmmitedRay.DestinationColor.Intensity / maxIntensity;
                float weight = hittingRays[i].EmmitedRay.DestinationColor.Intensity / maxIntensity;

                if (!float.IsNormal(weight) && weight != 0f)
                    weight = 0f;

                if (!float.IsNormal(lightOnlyWeight) && weight != 0f)
                    lightOnlyWeight = 0f;

                if (!comingFromLightSource)
                    weight *= Reflectiveness;

                Vector3D normal = CalculateNormal(shape, pointOfContact);
                float dot = Vector3D.Dot(normal, hittingRays[i].EmmitedRay.Direction * -1f);

                if (dot < 0)
                    dot = 0;

                totalColor += ((RawRTColor) hittingRays[i].EmmitedRay.DestinationColor) * weight * dot;

               if (comingFromLightSource)
                    lightOnlyColor += ((RawRTColor) hittingRays[i].EmmitedRay.DestinationColor) * dot * lightOnlyWeight;
            }

            RawRTColor refOnlyColor = totalColor - lightOnlyColor;

            float finalIntensity = lightOnlyIntensity + (totalIntensity - lightOnlyIntensity) * Reflectiveness;
            RawRTColor finalColor = lightOnlyColor + (totalColor - lightOnlyColor) * Reflectiveness;

//            float finalIntensity = totalIntensity;
//            RawRTColor finalColor = totalColor;

            if (finalIntensity <= 0f)
                return RTColor.Black;

            if (MainTexture != null)
            {
                float rgbMultiplier = 1f / 255f;

                var rgb = MainTexture.GetColorFromUV(shape.CalculateUV(pointOfContact));
                RTColor textureColor = new RTColor(0f, rgb.R * rgbMultiplier, rgb.G * rgbMultiplier, rgb.B * rgbMultiplier);

                finalColor = (finalColor + (((RawRTColor)textureColor) * TextureStrength)) / (1f + TextureStrength);
            }

            return new RTColor(finalIntensity * (1f - Absorbance), finalColor.R, finalColor.G, finalColor.B);
        }

        public override Ray[] GetOutgoingRays(Shape shape, Ray tracingRay, Vector3D pointOfContact)
        {
            return new Ray[] { new Ray(pointOfContact, RTMath.CalculateReflectedRayDirection(tracingRay.Direction, shape.CalculateNormal(pointOfContact))) };

            if (NormalMap != null)
                return new Ray[] { new Ray(pointOfContact, RTMath.CalculateReflectedRayDirection(tracingRay.Direction, CalculateNormal(shape, pointOfContact))) };
            else
                return base.GetOutgoingRays(shape, tracingRay, pointOfContact);
        }
    }
}