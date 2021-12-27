namespace RayTracing
{
    public class MeshTriangle : Shape
    {
        private readonly Vector3D DefaultNormal;

        public readonly bool HasCustomNormals = false;
        public readonly bool HasCustomUVs = false;

        public readonly Triangle VertexTriangle;
        public readonly Triangle? NormalTriangle;
        public readonly Triangle? UVTriangle;

        public MeshTriangle(Triangle vertices, Triangle? normals, Triangle? uvs, ShapeShader shader, Vector3D? defaultNormal = null) : base(vertices.Centroid, shader)
        {
            this.VertexTriangle = vertices;

            if (defaultNormal.HasValue)
            {
                HasCustomNormals = true;
                DefaultNormal = defaultNormal.Value;
            }

            DefaultNormal = vertices.Normal;

            if (normals.HasValue)
            {
                HasCustomNormals = true;
                NormalTriangle = normals.Value;
            }

            if (uvs.HasValue)
            {
                HasCustomUVs = true;
                UVTriangle = uvs.Value;
            }
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subShape)
        {
            subShape = null;
            return RTMath.RayTriangleIntersectionPoint(ray, VertexTriangle);
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            if (!HasCustomNormals)
                return DefaultNormal;

            if (NormalTriangle.HasValue)
            {
                Vector3D contribution = VertexTriangle.CalculateBarycentricPoint(pointOfContact);

                return NormalTriangle.Value[0] * contribution[0] + NormalTriangle.Value[1] * contribution[1] + NormalTriangle.Value[2] * contribution[2];
            }
            else
                return DefaultNormal;

        }

        public override Vector2D CalculateUV(Shape subShape, Vector3D pointOfContact)
        {
            if (!HasCustomUVs)
                throw new InvalidOperationException("Triangle does not have valid UVs.");

            Vector3D contribution = VertexTriangle.CalculateBarycentricPoint(pointOfContact);

            Vector2D uv = UVTriangle.Value[0] * contribution[0] + UVTriangle.Value[1] * contribution[1] + UVTriangle.Value[2] * contribution[2];

            return uv;
        }

        public Vector3D GetVertex(int index)
        {
            return VertexTriangle[index];
        }
    }
}