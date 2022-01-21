namespace RayTracing
{
    public abstract class LightSource : WorldObject
    {
        public override int TypeID => (int) TypeIDs.Light;

        private RTColor emmisionColor;

        public RTColor LightColor => emmisionColor;

        public LightSource(Transfomration transform, RTColor emmisionColor) : base(transform)
        {
            this.emmisionColor = emmisionColor;
        }

        public abstract ColoredRay ReachingRays(World world, Vector3D point);
    }
}