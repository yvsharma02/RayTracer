namespace RayTracing
{
    public class PlaneShape : Shape
    {
        private System.Func<RTRay[][], Vector3D, RTColor> BouceColorCalculator;

        private Vector3D normalVector;

        private Vector3D firstAxis;
        private Vector3D secondAxis;

        // Size extends in both positive and negative direction.
        public PlaneShape(Vector3D center, Vector3D firstAxisSize, Vector3D secondAxisSize, System.Func<RTRay[][], Vector3D, RTColor> colorProcessor) : base(center)
        {
            if (!Vector3D.ArePerpendicular(firstAxisSize, secondAxisSize))
                throw new ArgumentException("Both axis should be perpendicular");

            this.BouceColorCalculator = colorProcessor;

            this.firstAxis = firstAxisSize;
            this.secondAxis = secondAxisSize;

            this.normalVector = Vector3D.Cross(firstAxisSize, secondAxisSize).Normalize();
        }

        public override RTColor CalculateBouncedRayColor(RTRay[][] incidentRays, Vector3D outRayDir)
        {
            return BouceColorCalculator(incidentRays, outRayDir);
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return normalVector;
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            return MathUtil.RayPlaneContact(ray, firstAxis, secondAxis, Position, true);
        }

    }
}