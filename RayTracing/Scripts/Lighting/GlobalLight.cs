namespace RayTracing
{
    public class GlobalLight : LightSource
    {
        public readonly Vector3D Direction;

        // axis includes length too
        public GlobalLight(Transfomration transform, Vector3D direction, RTColor color) : base(transform, color)
        {
            this.Direction = direction.Normalize();
        } 

        public override ColoredRay ReachingRays(World world, Vector3D point)
        {
            Shape shape = world.ClosestShapeHit(new Ray(point, Direction * -1f), out Vector3D poc);

            if (shape == null)
                return new ColoredRay(Direction * float.NegativeInfinity, Direction, point, LightColor, LightColor);
            else
                return new ColoredRay(Direction * float.NegativeInfinity, Direction, point, RTColor.Black, RTColor.Black);
        }
    }
}