namespace RayTracing
{
    public class SphereShape : Shape
    {
        public readonly float Radius;
        public Vector3D Center
        {
            get
            {
                return Position;
            }
        }

        public SphereShape(Vector3D center, float radius, ShapeShader shader) : base(center, shader)
        {
            this.Radius = radius;
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return (pointOfContact - Position).Normalize();
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            return RTMath.RaySpherePointOfContact(ray, Position, Radius);
        }
    }
}
