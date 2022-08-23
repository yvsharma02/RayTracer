namespace RayTracing
{
    public class Mesh : Shape
    {
        private Bounds bounds;
        public override Bounds BoundaryBox => bounds;

        private MeshTriangle[] meshTriangles;

        public Mesh(Transformation transform, ShapeShader shader, Vector3D[] vertices, int[] triangles, Vector3D[] normals = null, int[] normalTriangles = null, Vector2D[] uvs = null, int[] uvTriangles = null) : base(transform, null)
        {
            if (triangles.Length % 3 != 0)
                throw new ArgumentException("triangles array length should be a multiple of 3.");

            if (uvTriangles != null && uvTriangles.Length % 3 != 0)
                throw new ArgumentException("Length of uv triangles array should be a multiple of 3.");
            if (normalTriangles != null && normalTriangles.Length % 3 != 0)
                throw new ArgumentException("Length of normal triangles array should be a multiple of 3.");

            this.meshTriangles = new MeshTriangle[triangles.Length / 3];

            int t = 0;

            for (int i = 0; i < meshTriangles.Length; i++)
            {
                bool validNormals = normals != null && normalTriangles != null && t + 2 < normalTriangles.Length;
                bool validUVs = uvs != null && uvTriangles != null && t + 2 < uvTriangles.Length;

                bool identicalNormals = validNormals ? normalTriangles[t] == normalTriangles[t + 1] && normalTriangles[t] == normalTriangles[t + 2] : false; 

                Triangle vertexTriangle = new Triangle(vertices[triangles[t]], vertices[triangles[t + 1]], vertices[triangles[t + 2]]);
                Triangle? uvTriangle = validUVs ? new Triangle(uvs[uvTriangles[t]], uvs[uvTriangles[t + 1]], uvs[uvTriangles[t + 2]]) : null;
                Triangle? normalTriangle = validNormals ? new Triangle(normals[normalTriangles[t]], normals[normalTriangles[t + 1]], normals[normalTriangles[t + 2]]) : null;

                meshTriangles[i] = new MeshTriangle(new Transformation(), vertexTriangle, identicalNormals ? null : normalTriangle, uvTriangle, shader, null, false);

                t += 3;
            }

            SetTransform(transform, true);
        }

        protected override void ApplyTransform()
        {
            int t = 0;

            if (meshTriangles == null)
                return;

            for (int i = 0; i < meshTriangles.Length; i++)
                meshTriangles[i].SetLocalTransform(transform);

            RecalculateBounds();
        
            base.ApplyTransform();
        }

        private void RecalculateBounds()
        {
            float[] minBounds = new float[] { float.MaxValue, float.MaxValue, float.MaxValue };
            float[] maxBounds = new float[] { float.MinValue, float.MinValue, float.MinValue };

            for (int i = 0; i < meshTriangles.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3D vertex = meshTriangles[i].VertexTriangle[j];

                    for (int k = 0; k < 3; k++)
                    {
                        if (vertex[k] < minBounds[k])
                            minBounds[k] = vertex[k] - Vector3D.EPSILON;
                        if (vertex[k] > maxBounds[k])
                            maxBounds[k] = vertex[k] + Vector3D.EPSILON;
                    }
                }
            }

            bounds = new Bounds(new Vector3D(minBounds[0], minBounds[1], minBounds[2]), new Vector3D(maxBounds[0], maxBounds[1], maxBounds[2]));
        }

        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            throw new InvalidOperationException("Subshape must be useed to calculate norma.");
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out Shape subshape)
        {
            Vector3D? boundsPOC = RTMath.RayBoundsContact(ray, BoundaryBox.LowerBounds, BoundaryBox.UpperBounds);

            if (!boundsPOC.HasValue)
            {
                subshape = null;
                return null;
            }

            Shape closestShape = null;
            float closestShapeDistSq = float.MaxValue;
            Vector3D? closestShapePOC = null;

            for (int i = 0; i < meshTriangles.Length; i++)
            {
                Vector3D poc;
                if (meshTriangles[i].HitsRay(ray, out poc, out Shape _))
                {
                    float distSq = Vector3D.DistanceSq(ray.Origin, poc);

                    if (distSq < closestShapeDistSq)
                    {
                        closestShapeDistSq = distSq;
                        closestShape = meshTriangles[i];
                        closestShapePOC = poc;
                    }
                }
            }

            subshape = closestShape;
            return closestShapePOC;
        }

        public override Vector2D CalculateUV(Vector3D pointOfContact)
        {
            throw new InvalidOperationException("Subshape must be used to calculate UV");
        }
    }
}