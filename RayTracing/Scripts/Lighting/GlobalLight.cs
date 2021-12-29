namespace RayTracing
{
    public class GlobalLight : LightSource
    {
        private const float GLOBAL_MIN_MULTIPLIER = 0;

        public readonly Vector3D Direction;

        private Int2D rayCount;

        // axis includes length too
        public GlobalLight(Transfomration transform, Vector3D direction, RTColor color) : base(transform, color)
        {
            this.Direction = direction.Normalize();
        } 

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subLight)
        {
            subLight = null;
            return ray.Origin + ray.Direction * float.PositiveInfinity;
        }

        public override ColoredRay[] ReachingRays(World world, Vector3D point)
        {
            Shape shape = world.ClosestShapeHit(new Ray(transform.Position, Direction * -1f), out Vector3D poc);

            if (shape == null)
                return new ColoredRay[] { new ColoredRay(Direction * float.NegativeInfinity, Direction, LightColor, point) };
            else
                return new ColoredRay[0];
        }
    }
}