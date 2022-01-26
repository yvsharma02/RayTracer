namespace RayTracing
{
    public class PointLight : LightSource
    {
        public override int TypeID => (int)RayTracing.TypeID.PointLight;

        private const float DEFAULT_HIT_RADIUS = 0.1f;

        public readonly float HitRadius;

        public Vector3D Position { get => Transform.Position; }

        public PointLight(RTColor color, Transformation transform, float hitRadius = DEFAULT_HIT_RADIUS) : base(transform, color)
        {
            this.HitRadius = hitRadius;
        }

        public override ColoredRay GetReachingRays(World world, Vector3D point)
        {
            Vector3D dirFromPointToLight = Position - point;

            Shape closestShapeToPoint = world.ClosestShapeHit(new Ray(point, dirFromPointToLight), out Vector3D poc);
            if (closestShapeToPoint == null || poc.DistanceFromSq(point) >= poc.DistanceFromSq(Position))
                return new ColoredRay(Position, dirFromPointToLight * -1f, point, LightColor, new RTColor(LightColor.Intensity / Vector3D.DistanceSq(point, Position), LightColor.AbsoluteR, LightColor.AbsoluteG, LightColor.AbsoluteB));
            else
                return new ColoredRay(Position, dirFromPointToLight * -1f, point, RTColor.Black, RTColor.Black);
        }
    }
}
