namespace RayTracing
{
    public struct RTRay
    {
        public readonly RTColor SourceColor;
        public readonly Vector3D Origin;
        public readonly Vector3D Direction;

        public readonly Vector3D? PointOfContact;

        public RTRay(Vector3D origin, Vector3D dir, RTColor emmitedColor)
        {
            this.SourceColor = emmitedColor;
            this.Origin = origin;
            this.Direction = dir.Normalize();

            this.PointOfContact = null;
        }

        public RTRay(Vector3D origin, Vector3D dir, RTColor emmitedColor, Vector3D pointOfContact) : this(origin, dir, emmitedColor)
        { 
            this.PointOfContact = pointOfContact;
        }
    }
}