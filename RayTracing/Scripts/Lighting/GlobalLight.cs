namespace RayTracing
{
    public class GlobalLight : LightSource
    {
        public override int TypeID => (int)RayTracing.TypeID.SunLight;

        public readonly Vector3D Direction;

        // axis includes length too
        public GlobalLight(Transformation transform, Vector3D direction, RTColor color) : base(transform, color)
        {
            this.Direction = direction.Normalize();
        } 

        public override ColoredRay GetReachingRays(World world, Vector3D point)
        {
            Vector3D poc;
            Shape shape = world.ClosestShapeHit(new Ray(point, Direction * -1f), out poc);

            if (shape == null)
                return new ColoredRay(Direction * float.NegativeInfinity, Direction, point, LightColor, LightColor);
            else
                return new ColoredRay(Direction * float.NegativeInfinity, Direction, point, RTColor.Black, RTColor.Black);
        }
    }
}