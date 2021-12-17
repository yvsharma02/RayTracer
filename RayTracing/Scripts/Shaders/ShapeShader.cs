namespace RayTracing
{
    public abstract class ShapeShader
    {
        public abstract RTColor CalculateBounceColor(Shape shape, ColoredRay[][] hittingRays, Vector3D pointOfContact, Vector3D outgoingRayDir);

        public virtual Ray[] ReverseEmmitedRays(Shape shape, Ray reverseHitRay, Vector3D pointOfContact)
        {
            Vector3D normal = shape.CalculateNormal(null, pointOfContact);

            Vector3D reflectedDir = RTMath.CalculateReflectedRayDirection(reverseHitRay.DirectionReversed * -1f, normal);

            return new Ray[] { new Ray(pointOfContact + (normal * Vector3D.EPSILON), reflectedDir) };
        }
    }
}