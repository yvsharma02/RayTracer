namespace RayTracing
{
    public abstract class LightSource : WorldObject
    {
        public readonly RTColor LightColor;

        public LightSource(Vector3D position, RTColor sourceColor) : base(position)
        {
            this.LightColor = sourceColor;
        }

        public override bool IsLightSource => true;

        public abstract float CalculateMultiplier(Ray reverseRay);

        public abstract ColoredRay[] ReachingRays(World world, Vector3D point);
    }
}