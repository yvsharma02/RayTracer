namespace RayTracing
{
    public class PlaneShape : Shape
    {
        private System.Func<RTColor, Ray, Vector3D, RTColor> BouceColorCalculator;

        private Vector3D normalVector;

        private Vector3D firstAxis;
        private Vector3D secondAxis;

        // Size extends in both positive and negative direction.
        public PlaneShape(Vector3D center, Vector3D firstAxisSize, Vector3D secondAxisSize, System.Func<RTColor, Ray, Vector3D, RTColor> colorProcessor) : base(center)
        {
            if (!Vector3D.ArePerpendicular(firstAxisSize, secondAxisSize))
                throw new ArgumentException("Both axis should be perpendicular");

            this.normalVector = normalVector.Normalize();
            this.BouceColorCalculator = colorProcessor;

            this.firstAxis = firstAxisSize;
            this.secondAxis = secondAxisSize;

            this.normalVector = Vector3D.Cross(firstAxisSize, secondAxisSize).Normalize();
        }

        public override RTColor CalculateBouncedRayColor(RTColor sourceColor, Ray sourceRay, Vector3D pointOfContact)
        {
            return BouceColorCalculator(sourceColor, sourceRay, pointOfContact);
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return normalVector;
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            Vector3D o = ray.Origin;
            Vector3D p = Position;
            Vector3D n = normalVector;
            Vector3D d = ray.Direction;

            float lamda = Vector3D.Dot(p - o, n) / Vector3D.Dot(d, n);

            if (lamda >= 0)
            {
                Vector3D poc = o + (d * lamda);

                Vector3D r = poc - Position;

                float firstAxisProjectionLength = Vector3D.Dot(r, firstAxis) / firstAxis.Magnitude();
                float secondAxisProjectionLength = Vector3D.Dot(r, secondAxis) / secondAxis.Magnitude();

                if (firstAxisProjectionLength < 0 || secondAxisProjectionLength < 0)
                    return null;

                if (firstAxisProjectionLength <= firstAxis.Magnitude() && secondAxisProjectionLength <= secondAxis.Magnitude())
                    return poc;

                return null;

            }
            else
                return null;
        }

    }
}