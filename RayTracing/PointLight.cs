namespace RayTracing
{
    public class PointLight : LightSource
    {
        private const float DEFAULT_HIT_RADIUS = 0.1f;

        public readonly float HitRadius;

        public Vector3D Center
        {
            get
            {
                return Position;
            }
        }

        public PointLight(RTColor color, Vector3D position, float hitRadius = DEFAULT_HIT_RADIUS) : base(position, color)
        {
            this.HitRadius = hitRadius;
        }

        public override float CalculateMultiplier(Ray ray)
        {
            return 1f;
        }

        public override bool HitsRay(Ray ray, out Vector3D pointOfContact)
        {
            return MathUtil.RayHitsSphere(ray, Position, HitRadius, out pointOfContact);
        }

        public override bool HitsRay(Ray ray)
        {
            return MathUtil.RayHitsSphere(ray, Position, HitRadius);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            return MathUtil.RaySpherePointOfContact(ray, Position, HitRadius);
        }

        public override RTRay[] ReachingRays(World world, Vector3D point)
        {
            Vector3D dirFromPointToLight = Position - point;

            Shape closestShapeToPoint = world.ClosestShapeHit(new Ray(point, dirFromPointToLight), out Vector3D poc);
            if (closestShapeToPoint == null || poc.DistanceFrom(point) >= poc.DistanceFrom(Position))
                return new RTRay[] { new RTRay(Position, dirFromPointToLight * -1f, LightColor, point) };
            else
                return new RTRay[] { new RTRay(Position, dirFromPointToLight * -1f, RTColor.Black, point) };
        }
    }
}
