namespace RayTracing
{
    public class PointLight : LightSource
    {
        private const float DEFAULT_HIT_RADIUS = 0.1f;

        public readonly float HitRadius;

        public Vector3D Position { get => Transform.Position; }

        public PointLight(RTColor color, Transfomration transform, float hitRadius = DEFAULT_HIT_RADIUS) : base(transform, color)
        {
            this.HitRadius = hitRadius;
        }

        public override bool HitsRay(Ray ray, out Vector3D pointOfContact, out WorldObject subLight)
        {
            subLight = null;
            return RTMath.RayHitsSphere(ray, Position, HitRadius, out pointOfContact);
        }

        public override bool HitsRay(Ray ray)
        {
            return RTMath.RayHitsSphere(ray, Position, HitRadius);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subLight)
        {
            subLight = null;
            return RTMath.RaySpherePointOfContact(ray, Position, HitRadius);
        }

        public override ColoredRay[] ReachingRays(World world, Vector3D point)
        {
            Vector3D dirFromPointToLight = Position - point;

            Shape closestShapeToPoint = world.ClosestShapeHit(new Ray(point, dirFromPointToLight), out Vector3D poc);
            if (closestShapeToPoint == null || poc.DistanceFromSq(point) >= poc.DistanceFromSq(Position))
                return new ColoredRay[] { new ColoredRay(Position, dirFromPointToLight * -1f, LightColor, point) };
            else
                return new ColoredRay[] { new ColoredRay(Position, dirFromPointToLight * -1f, RTColor.Black, point) };
        }
    }
}
