namespace RayTracing
{
    public class PointLight : LightSource
    {
        public const float DEFAULT_DISTANCE_SCALE = 0.001f;

        public readonly float DistanceScale;

        public override int TypeID => (int)RayTracing.TypeID.PointLight;

        public Vector3D Position { get => Transform.Position; }

        public PointLight(RTColor color, Transformation transform, float distanceScale = DEFAULT_DISTANCE_SCALE) : base(transform, color)
        {
            this.DistanceScale = distanceScale;
        }

        public override ColoredRay GetReachingRays(World world, Vector3D point)
        {
            Vector3D dirFromLightToPoint = point - Position;

            Shape closestShapeToPoint = world.ClosestShapeHit(new Ray(Position, point - Position), out Vector3D poc);

            if (closestShapeToPoint == null || Position.DistanceFromSq(poc) >= Position.DistanceFromSq(point))
            {
                float distance = Vector3D.Distance(point, Position) * DistanceScale;
                return new ColoredRay(Position, dirFromLightToPoint * 1f, point, LightColor, new RTColor(LightColor.Intensity / distance * distance, LightColor.R, LightColor.G, LightColor.B));
            }
            else
                return null;
        }
    }
}
