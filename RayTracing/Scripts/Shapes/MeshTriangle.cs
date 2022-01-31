namespace RayTracing
{
    public class MeshTriangle : Shape
    {
        private Vector3D DefaultNormal;

        public readonly bool HasCustomNormals = false;
        public readonly bool HasCustomUVs = false;

        private Triangle vertexTriangle;
        private Triangle? normalTriangle;
        private Triangle? uvTriangle;

        public Triangle VertexTriangle => vertexTriangle;
        public Triangle UVTriangle => uvTriangle.Value;
        public Triangle NormalTriangle => normalTriangle.Value;

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
                normalTriangle = normals.Value;
            }

            if (uvs.HasValue)
            {
                HasCustomUVs = true;
                uvTriangle = uvs.Value;
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
        
            if (normalTriangle.HasValue)
            {
                Vector3D[] normals = new Vector3D[3];

                for (int i = 0; i < 3; i++)
                    normals[i] = newTransform.Transform(oldTransform.InverseTransform(normalTriangle.Value[i], false, true, false), false, true, false).Normalize();

                normalTriangle = new Triangle(normals[0], normals[1], normals[2]);
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

            if (normalTriangle.HasValue)
            {
                Vector3D contribution = vertexTriangle.CalculateBarycentricPoint(pointOfContact);
                return (normalTriangle.Value[0] * contribution[0] + normalTriangle.Value[1] * contribution[1] + normalTriangle.Value[2] * contribution[2]).Normalize();
            }
            else
                return DefaultNormal;
        }

        public override Vector2D CalculateUV(Vector3D pointOfContact)
        {
            if (!HasCustomUVs)
                throw new InvalidOperationException("Triangle does not have valid UVs.");

            Vector3D contribution = vertexTriangle.CalculateBarycentricPoint(pointOfContact);
            Vector2D uv = uvTriangle.Value[0] * contribution[0] + uvTriangle.Value[1] * contribution[1] + uvTriangle.Value[2] * contribution[2];

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