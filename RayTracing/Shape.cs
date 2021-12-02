namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        public Shape(Vector3D position) : base(position)
        {

        }

        public abstract RTColor CalculateBouncedRayColor(RTColor sourceColor, Ray sourceRay, Vector3D pointOfContact);

        public abstract Vector3D CalculateNormal(Vector3D pointOfContact);

        public override bool IsLightSource => false;
    }
}