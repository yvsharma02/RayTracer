namespace RayTracing
{
    public class GlobalLight : LightSource
    {
        private const float GLOBAL_MIN_MULTIPLIER = 0;

        public readonly Vector3D Direction;

        private Vector3D firstAxis;
        private Vector3D secondAxis;

        private Int2D rayCount;

        // axis includes length too
        public GlobalLight(Vector3D position, Vector3D firstAxis, Vector3D secondAxis, Int2D rayCount, RTColor color) : base(position, color)
        {
            if (!Vector3D.ArePerpendicular(firstAxis, secondAxis))
                throw new ArgumentException("First and Second axis should be perpendicular");

            this.Direction = Vector3D.Cross(firstAxis, secondAxis).Normalize();
            this.firstAxis = firstAxis;
            this.secondAxis = secondAxis;
            this.rayCount = rayCount;
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

        public override ColoredRay[] ReachingRays(World world, Vector3D point)
        {
            Vector3D axisIntersection = point - (firstAxis / 2f) - (secondAxis / 2f);

//            RTRay[] rays = new RTRay[rayCount.x * rayCount.y];
            int totalRayCount = rayCount.x * rayCount.y;
            int hits = 0;

            for (int i = 0; i < rayCount.x; i++)
            {
                for (int j = 0; j < rayCount.y; j++)
                {
                    Vector3D rayOrigin = axisIntersection + firstAxis * ((float)i / rayCount.x) + secondAxis * ((float)j / rayCount.y);

                    Vector3D contactPoint;
                    // Shooting a ray from the point towards the direction of light. If it doesn't hit any shape, it means global light is directly hitting the surface.
                    Shape shape = world.ClosestShapeHit(new Ray(rayOrigin, Direction * -1f), out contactPoint);

                    //                    RTColor clr = Vector3D.Distance(contactPoint, point) <= Vector3D.EPSILON ? LightColor : ShadowColor;
                    //                    rays[i * rayCount.x + j] = new RTRay(origin, Direction, clr);
                    if (shape == null)
                        hits += 1;
                }
            }

            ColoredRay[] rays = new ColoredRay[1];
            rays[0] = new ColoredRay(Direction * float.NegativeInfinity, Direction, new RTColor(LightColor.Intensity * ((float) hits / totalRayCount), LightColor.G, LightColor.B, LightColor.B), point);

            return rays;
        }
    }
}