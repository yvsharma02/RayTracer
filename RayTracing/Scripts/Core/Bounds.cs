namespace RayTracing
{
    public struct Bounds
    {
        public readonly Vector3D LowerBounds;
        public readonly Vector3D UpperBounds;

        public Bounds(Vector3D lower, Vector3D upper)
        {
            this.LowerBounds = lower;
            this.UpperBounds = upper;
        }

    }
}