namespace RayTracing
{
    public class Ray
    {
        public readonly Vector3D origin;
        public readonly Vector3D direction;

        public Ray(Vector3D origin, Vector3D direction)
        {
            this.origin = origin;
            this.direction = direction.Normalize();
        }
    }
}
