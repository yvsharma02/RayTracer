namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        public Shape(Vector3D position) : base(position)
        {

        }

        public abstract RTColor CalculateBouncedRayColor(RTRay[][] hittingRays, Vector3D outRayDir);

        public abstract Vector3D CalculateNormal(Vector3D pointOfContact);

        public override bool IsLightSource => false;
    }
}