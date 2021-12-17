namespace RayTracing
{
    public struct ColoredRay
    {
        public readonly RTColor DestinationColor;
        public readonly RTColor SourceColor;
        public readonly Vector3D Origin;
        public readonly Vector3D Direction;

        public readonly Vector3D PointOfContact;

        public ColoredRay(Vector3D origin, Vector3D dir, RTColor emmitedColor, Vector3D pointOfContact)
        {
            this.SourceColor = emmitedColor;
            this.Origin = origin;
            this.Direction = dir.Normalize();

            this.PointOfContact = pointOfContact;

            double finalIntensity = 0f;

            if (!Origin.IsInfinity)
            {
                float dist = Origin.DistanceFrom(pointOfContact);

                finalIntensity = SourceColor.Intensity / (dist * dist);

                if (finalIntensity >= RTColor.MAX_INTENSITY)
                    finalIntensity = RTColor.MAX_INTENSITY;
            }
            else
                finalIntensity = SourceColor.Intensity;

            this.DestinationColor = new RTColor(finalIntensity, SourceColor.R, SourceColor.G, SourceColor.B);
        }
    }
}