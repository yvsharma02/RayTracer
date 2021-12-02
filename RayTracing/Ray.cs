namespace RayTracing
{
    public struct Ray
    {
        public readonly Vector3D Origin;
        public readonly Vector3D Direction;
        public Vector3D DirectionReversed { get => Direction * -1f; }
        
        public Ray(Vector3D origin, Vector3D direction)
        {
            this.Origin = origin;
            this.Direction = direction.Normalize();
        }
        public Ray ReverseDirection()
        {
            return new Ray(Origin, Direction * -1f);
        }
    }
}
