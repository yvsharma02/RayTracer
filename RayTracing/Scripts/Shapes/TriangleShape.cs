namespace RayTracing
{
    public class TriangleShape : Shape
    {
        private Vector3D[] vertices;

        public TriangleShape(Vector3D pt1, Vector3D pt2, Vector3D pt3, ShapeShader shader) : base((pt1 + pt2 + pt3) / 3f, shader)
        {
            this.vertices = new Vector3D[] { pt1, pt2, pt3 };
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subShape)
        {
            subShape = null;
            return RTMath.RayTriangleIntersectionPoint(ray, vertices[0], vertices[1], vertices[2]);
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            return RTMath.CalculateTriangleNormal(vertices[0], vertices[1], vertices[2]);
        }

        public Vector3D GetVertex(int index)
        {
            return vertices[index];
        }
    }
}