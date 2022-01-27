namespace RayTracing
{
    public class MeshTriangle : Shape
    {
        private Vector3D DefaultNormal;

        public readonly bool HasCustomNormals = false;
        public readonly bool HasCustomUVs = false;

        private Triangle vertexTriangle;
        private Triangle? NormalTriangle;
        private Triangle? UVTriangle;

        public Triangle VertexTriangle => vertexTriangle;

        public MeshTriangle(Transformation transform, Triangle vertices, Triangle? normals, Triangle? uvs, ShapeShader shader, Vector3D? defaultNormal = null, bool applyTransformImmediately = true) : base(transform, shader)
        {
            this.vertexTriangle = vertices;

            if (defaultNormal.HasValue)
            {
                HasCustomNormals = true;
                DefaultNormal = defaultNormal.Value;
            }
            else
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

            SetTransform(transform, applyTransformImmediately);
        }

        protected override void ApplyTransform()
        {
            Transformation newTransform = transform;

            Vector3D[] vertices = new Vector3D[3];
            for (int i = 0; i < 3; i++)
                vertices[i] = newTransform.Transform(oldTransform.InverseTransform(vertexTriangle[i]));

            vertexTriangle = new Triangle(vertices[0], vertices[1], vertices[2]);
        
            if (NormalTriangle.HasValue)
            {
                Vector3D[] normals = new Vector3D[3];

                for (int i = 0; i < 3; i++)
                    normals[i] = newTransform.Transform(oldTransform.InverseTransform(NormalTriangle.Value[i], false, true, false), false, true, false).Normalize();

                NormalTriangle = new Triangle(normals[0], normals[1], normals[2]);
            }
            DefaultNormal = newTransform.Transform(oldTransform.InverseTransform(DefaultNormal, false, true, false), false, true, false).Normalize();

            base.ApplyTransform();
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out Shape subShape)
        {
            subShape = null;
            return RTMath.RayTriangleIntersectionPoint(ray, vertexTriangle);
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            if (!HasCustomNormals)
                return DefaultNormal;

            if (NormalTriangle.HasValue)
            {
                Vector3D contribution = vertexTriangle.CalculateBarycentricPoint(pointOfContact);
                return (NormalTriangle.Value[0] * contribution[0] + NormalTriangle.Value[1] * contribution[1] + NormalTriangle.Value[2] * contribution[2]).Normalize();
            }
            else
                return DefaultNormal;
        }

        public override Vector2D CalculateUV(Vector3D pointOfContact)
        {
            if (!HasCustomUVs)
                throw new InvalidOperationException("Triangle does not have valid UVs.");

            Vector3D contribution = vertexTriangle.CalculateBarycentricPoint(pointOfContact);
            Vector2D uv = UVTriangle.Value[0] * contribution[0] + UVTriangle.Value[1] * contribution[1] + UVTriangle.Value[2] * contribution[2];

            float finalUVx = uv.x > 1 ? 1 : uv.x < 0 ? 0 : uv.x;
            float finalUVy = uv.y > 1 ? 1 : uv.y < 0 ? 0 : uv.y;

            return new Vector2D(finalUVx, finalUVy);
        }

        public Vector3D GetVertex(int index)
        {
            return vertexTriangle[index];
        }
    }
}