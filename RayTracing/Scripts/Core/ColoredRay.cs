namespace RayTracing
{
    public class ColoredRay
    {
        public readonly WorldObject ImmediateSource;
        public readonly RTColor DestinationColor;
        public readonly RTColor SourceColor;

        public readonly Vector3D Origin;
        public readonly Vector3D Direction;
        public readonly Vector3D PointOfContact;

        public ColoredRay(Vector3D origin, Vector3D dir, Vector3D pointOfContact, RTColor srcColor, RTColor destinationColor, WorldObject immediateSource)
        {
            this.ImmediateSource = immediateSource;
            this.SourceColor = srcColor;
            this.Origin = origin;
            this.Direction = dir.Normalize();
            this.PointOfContact = pointOfContact;
            this.DestinationColor = destinationColor;
        }
    }
}