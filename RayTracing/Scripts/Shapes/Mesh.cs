namespace RayTracing
{
    public class Mesh : Shape
    {
        public readonly Vector3D LowerBounds;
        public readonly Vector3D UpperBounds;

        private MeshTriangle[] triangleShapes;

        public Mesh(Vector3D position, ShapeShader shader, Vector3D[] vertices, int[] triangles, Vector3D[] normals = null, int[] normalTriangles = null, Vector2D[] uvs = null, int[] uvTriangles = null) : base(position, shader)
        {
            if (triangles.Length % 3 != 0)
                throw new ArgumentException("triangles array length should be a multiple of 3.");

            this.triangleShapes = new MeshTriangle[triangles.Length / 3];

            float[] minBounds = new float[] { float.MaxValue, float.MaxValue, float.MaxValue };
            float[] maxBounds = new float[] { float.MinValue, float.MinValue, float.MinValue };

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3D vertex = vertices[i];

                for (int j = 0; j < 3; j++)
                {
                    if (vertex[j] < minBounds[j])
                        minBounds[j] = vertex[j] - Vector3D.EPSILON;
                    if (vertex[j] > maxBounds[j])
                        maxBounds[j] = vertex[j] + Vector3D.EPSILON;
                }
            }

            LowerBounds = new Vector3D(minBounds[0], minBounds[1], minBounds[2]);
            UpperBounds = new Vector3D(maxBounds[0], maxBounds[1], maxBounds[2]);

            if (uvTriangles != null && uvTriangles.Length % 3 != 0)
                throw new ArgumentException("Length of uv triangles array should be a multiple of 3.");
            if (normalTriangles != null && normalTriangles.Length % 3 != 0)
                throw new ArgumentException("Length of normal triangles array should be a multiple of 3.");

            int t = 0;

            for (int i = 0; i < triangleShapes.Length; i++)
            {
                bool validNormals = normals != null && normalTriangles != null && t + 2 < normalTriangles.Length;
                bool validUVs = uvs != null && uvTriangles != null && t + 2 < uvTriangles.Length;

                bool identicalNormals = validNormals ? normalTriangles[t] == normalTriangles[t + 1] && normalTriangles[t] == normalTriangles[t + 2] : false; 

                Triangle vertexTriangle = new Triangle(vertices[triangles[t]], vertices[triangles[t + 1]], vertices[triangles[t + 2]]);
                Triangle? uvTriangle = validUVs ? new Triangle(uvs[uvTriangles[t]], uvs[uvTriangles[t + 1]], uvs[uvTriangles[t + 2]]) : null;
                Triangle? normalTriangle = validNormals ? new Triangle(normals[normalTriangles[t]], normals[normalTriangles[t + 1]], normals[normalTriangles[t + 2]]) : null;

                Vector3D? defaultNormal = identicalNormals ? normals[normalTriangles[t]] : null;

                triangleShapes[i] = new MeshTriangle(vertexTriangle, identicalNormals ? null : normalTriangle, uvTriangle, shader, defaultNormal);

                t += 3;
            }
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            return shape.CalculateNormal(null, pointOfContact);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subshape)
        {
            Vector3D? boundsPOC = RTMath.RayBoundsContact(ray, LowerBounds, UpperBounds);

            if (!boundsPOC.HasValue)
            {
                subshape = null;
                return null;
            }

            Shape closestShape = null;
            float closestShapeDist = float.MaxValue;
            Vector3D? closestShapePOC = null;

            for (int i = 0; i < triangleShapes.Length; i++)
            {
                Vector3D poc;
                if (triangleShapes[i].HitsRay(ray, out poc, out WorldObject _))
                {
                    float dist = Vector3D.Distance(ray.Origin, poc);

                    if (dist < closestShapeDist)
                    {
                        closestShapeDist = dist;
                        closestShape = triangleShapes[i];
                        closestShapePOC = poc;
                    }
                }
            }

            subshape = closestShape;
            return closestShapePOC;
        }

        public override Vector2D CalculateUV(Shape shape, Vector3D pointOfContact)
        {
            MeshTriangle triangle = (MeshTriangle)shape;

            return triangle.CalculateUV(null, pointOfContact);

        }
    }
}