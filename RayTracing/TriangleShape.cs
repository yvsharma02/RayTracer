namespace RayTracing
{
    public class TriangleShape : Shape
    {
        private Func<RTRay[][], Vector3D, RTColor> bounceColorCalculator;

        private Vector3D[] vertices;

        public TriangleShape(Vector3D pt1, Vector3D pt2, Vector3D pt3, Func<RTRay[][], Vector3D, RTColor> bounceClrCalculator) : base((pt1 + pt2 + pt3) / 3f)
        {
            this.vertices = new Vector3D[] { pt1, pt2, pt3 };
            this.bounceColorCalculator = bounceClrCalculator;
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            Vector3D? poc = MathUtil.RayPlaneContact(ray, vertices[1] - vertices[0], vertices[2] - vertices[1], vertices[0], false);

            if (!poc.HasValue)
                return null;

            Vector3D centroid = Position;
            
            Vector3D[] lineDirs = new Vector3D[] { vertices[1] - vertices[0], vertices[2] - vertices[1], vertices[0] - vertices[2] };

            Vector3D[] validIntersections = new Vector3D[2];
            int c = 0;

            for (int i = 0; i < 3; i++)
            {
                Vector3D? intersection = MathUtil.LineLineIntersectionPoint(vertices[i], lineDirs[i], poc.Value, centroid - poc.Value);
                if (intersection.HasValue && MathUtil.LiesOnLineInBetweenPoints(intersection.Value, vertices[i], vertices[(i + 1) % 3]))
                    validIntersections[c++] = intersection.Value;
            }

            return (!MathUtil.LiesOnLineInBetweenPoints(poc.Value, validIntersections[0], validIntersections[1])) ? null: poc.Value;
        }

        public override RTColor CalculateBouncedRayColor(RTRay[][] hittingRays, Vector3D outRayDir)
        {
            return bounceColorCalculator(hittingRays, outRayDir);
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return Vector3D.Cross(vertices[2] - vertices[1], vertices[1] - vertices[0]);
        }
    }
}