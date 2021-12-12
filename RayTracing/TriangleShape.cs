namespace RayTracing
{
    public class TriangleShape : Shape
    {
        private Vector3D[] vertices;

        public TriangleShape(Vector3D pt1, Vector3D pt2, Vector3D pt3, ShapeShader shader) : base((pt1 + pt2 + pt3) / 3f, shader)
        {
            this.vertices = new Vector3D[] { pt1, pt2, pt3 };
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            Vector3D? poc = RTMath.RayPlaneContact(ray, vertices[1] - vertices[0], vertices[2] - vertices[1], vertices[0], false);

            if (!poc.HasValue)
                return null;

            Vector3D centroid = Position;
            
            Vector3D[] lineDirs = new Vector3D[] { vertices[1] - vertices[0], vertices[2] - vertices[1], vertices[0] - vertices[2] };

            Vector3D[] intersections = new Vector3D[3];
            int intersectionCount = 0;
            int mostDistantIntersectionIndex = -1;

            Vector3D[] validIntersections = new Vector3D[2];
            // && MathUtil.LiesOnLineInBetweenPoints(intersection.Value, vertices[i], vertices[(i + 1) % 3])
            for (int i = 0; i < 3; i++)
            {
                Vector3D? intersection = RTMath.LineLineIntersectionPoint(vertices[i], lineDirs[i], poc.Value, centroid - poc.Value);
                if (intersection.HasValue)
                {
                    intersections[intersectionCount++] = intersection.Value;
                    if (mostDistantIntersectionIndex == -1 || intersections[mostDistantIntersectionIndex].DistanceFrom(centroid) < intersections[i].DistanceFrom(centroid))
                        mostDistantIntersectionIndex = i;
                
                }
            }

            int c = 0;
            for (int i = 0; i < intersectionCount; i++)
            {
                if (intersectionCount > 2 && i == mostDistantIntersectionIndex)
                    continue;
                validIntersections[c++] = intersections[i];
            }

            return (!RTMath.LiesOnLineInBetweenPoints(poc.Value, validIntersections[0], validIntersections[1])) ? null: poc.Value;
        }
        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            return Vector3D.Cross(vertices[2] - vertices[1], vertices[1] - vertices[0]);
        }
    }
}