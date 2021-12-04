namespace RayTracing
{
    public class GlobalLight : LightSource
    {
        private const float GLOBAL_MIN_MULTIPLIER = 0;

        public readonly Vector3D Direction;

        public GlobalLight(Vector3D position, Vector3D direction, RTColor color) : base(position, color)
        {
            this.Direction = direction.Normalize();
        } 

        public override float CalculateMultiplier(Ray reverseRay)
        {
            Vector3D dir = (reverseRay.Direction * -1f).Normalize();

            float multiplier = Vector3D.Dot(dir, Direction);

            if (multiplier <= GLOBAL_MIN_MULTIPLIER)
                return GLOBAL_MIN_MULTIPLIER;

            return multiplier;
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            return ray.Origin + ray.Direction * float.PositiveInfinity;
        }
    }
}