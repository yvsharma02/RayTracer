namespace RayTracing
{
    public abstract class ShapeShader
    {
        public abstract RTColor CalculateBounceColor(Shape shape, EmmisionChain[] hittingRays, Vector3D pointOfContact, Vector3D outgoingRayDir);

        public virtual Vector3D CalculateNormal(Shape shape, Vector3D poc) => shape.CalculateNormal(poc);

        public virtual Ray[] GetOutgoingRays(Shape shape, Ray tracingRay, Vector3D pointOfContact)
        {
            Vector3D normal = shape.CalculateNormal(pointOfContact);
            Vector3D reflectedDir = RTMath.CalculateReflectedRayDirection(tracingRay.Direction, normal);

            return new Ray[] { new Ray(pointOfContact, reflectedDir) };
        }

        public virtual RTColor CalculateDestinationColor(RTColor color, Vector3D src, Vector3D destination)
        {
            return RTColor.CalculateDropOffColor(color, src, destination);
        }
    }
}