namespace RayTracing
{
    public class SphereShape : Shape
    {
        private readonly Func<RTRay[][], Vector3D, RTColor> BounceColorCalculator;

        public readonly float Radius;
        public Vector3D Center
        {
            get
            {
                return Position;
            }
        }

        public SphereShape(Vector3D center, float radius, Func<RTRay[][], Vector3D, RTColor> bounceColorCalculator) : base(center)
        {
            this.BounceColorCalculator = bounceColorCalculator;
            this.Radius = radius;
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return (pointOfContact - Position).Normalize();
        }

        public override RTColor CalculateBouncedRayColor(RTRay[][] incidentRays, Vector3D outRayDir)
        {
            return BounceColorCalculator(incidentRays, outRayDir);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            return MathUtil.RaySpherePointOfContact(ray, Position, Radius);
        }
    }
}
