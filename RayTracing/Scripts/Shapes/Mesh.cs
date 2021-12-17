namespace RayTracing
{
    public class Mesh : Shape
    {
        private Vector3D lowerBounds;
        private Vector3D upperBounds;

        private TriangleShape[] triangleShapes;

        private Vector3D[] vertices;
        private int[] vertexTriangles;
        private Vector2D[] uvs;
        private int[] uvTriangles;

        public int UVTrianglesCount
        {
            get
            {
                return (uvTriangles.Length / 3);
            }
        }

        public int VertexTriangleCount
        {
            get
            {
                return (vertexTriangles.Length / 3);
            }
        }

        public Mesh(Vector3D position, ShapeShader shader, Vector3D[] vertices, int[] triangles) : base(position, shader)
        {
            if (triangles.Length % 3 != 0)
                throw new ArgumentException("triangles array length should be a multiple of 3.");

            this.triangleShapes = new TriangleShape[triangles.Length / 3];

            this.vertices = vertices.Clone<Vector3D>();
            this.vertexTriangles = triangles.Clone<int>();

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

            lowerBounds = new Vector3D(minBounds[0], minBounds[1], minBounds[2]);
            upperBounds = new Vector3D(maxBounds[0], maxBounds[1], maxBounds[2]);

            int t = 0;

            for (int i = 0; i < triangleShapes.Length; i++)
                triangleShapes[i] = new TriangleShape(vertices[triangles[t++]], vertices[triangles[t++]], vertices[triangles[t++]], shader);
        }

        public Mesh(Vector3D position, ShapeShader shader, Vector3D[] vertices, int[] triangles, Vector2D[] uvs, int[] uvTriangles) : this(position, shader, vertices, triangles)
        {
//            if (uvs.Length != vertices.Length)
//                throw new ArgumentException("Length of vertice and uv array should be the same.");

            if (uvTriangles.Length % 3 != 0)
                throw new ArgumentException("Length of uv triangles array should be a multiple of 3");

            this.uvs = uvs.Clone<Vector2D>();
            this.uvTriangles = uvTriangles.Clone<int>();
        }

        private int POCToVertexTriangleIndex(Vector3D poc)
        {
            for (int i = 0; i < VertexTriangleCount; i++)
            {
                Vector3D[] vertices = GetTriangleVertices(i);

                if (RTMath.LiesInsideTriangle(poc, true, vertices[0], vertices[1], vertices[2]))
                    return i;
            }
            return -1;
        }


        private int POCToUVTriangleIndex(Vector3D poc)
        {
            for (int i = 0; i < UVTrianglesCount; i++)
            {
                Vector3D[] vertices = GetUVTriangleVertices(i);

                if (RTMath.LiesInsideTriangle(poc, true, vertices[0], vertices[1], vertices[2]))
                    return i;
            }
            return -1;
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            return shape.CalculateNormal(shape, pointOfContact);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subshape)
        {
            Vector3D? boundsPOC = RTMath.RayBoundsContact(ray, lowerBounds, upperBounds);

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

        public override Int2D POCToTexturePixelIndex(Shape shape, Vector3D pointOfContact, Int2D TextureDimensions)
        {
            int triangleIndex = POCToUVTriangleIndex(pointOfContact);

            if (triangleIndex == -1)
                return new Int2D(-1, -1);

            Vector3D[] vertices = GetTriangleVertices(triangleIndex);
            Vector2D[] uvs = GetUVs(triangleIndex);

            Vector3D dir1 = vertices[1] - vertices[0];
            Vector3D dir2 = vertices[2] - vertices[1];
            Vector3D shiftedPoint = pointOfContact - vertices[0];

            float dir1part = (shiftedPoint.x * dir2.y - shiftedPoint.y * dir2.x) / (dir1.x * dir2.y - dir2.y * dir1.x);
            float dir2part = (shiftedPoint.x * dir1.y - shiftedPoint.y * dir1.x) / (dir1.y * dir2.x - dir2.x * dir1.y);

            Vector2D finalUV = (uvs[1] - uvs[0]) * dir1part + (uvs[2] - uvs[0]) * dir2part;

            return new Int2D((int)(TextureDimensions.x * finalUV.x), (int)(TextureDimensions.y * finalUV.y));
        }

        public Vector3D[] GetTriangleVertices(int triangleIndex)
        {
            if (triangleIndex >= vertexTriangles.Length / 3)
                throw new IndexOutOfRangeException("triangleIndex");

            triangleIndex *= 3;

            return new Vector3D[] { vertices[vertexTriangles[triangleIndex]], vertices[vertexTriangles[triangleIndex + 1]], vertices[vertexTriangles[triangleIndex + 2]] };
        }

        public Vector3D[] GetUVTriangleVertices(int triangleIndex)
        {
            if (triangleIndex >= uvTriangles.Length / 3)
                throw new IndexOutOfRangeException("triangleIndex");

            triangleIndex *= 3;

            return new Vector3D[] { vertices[uvTriangles[triangleIndex] - 1], vertices[uvTriangles[triangleIndex + 1] - 1], vertices[uvTriangles[triangleIndex + 2] - 1] };
        }

        public Vector2D[] GetUVs(int triangleIndex)
        {

            if (triangleIndex >= vertexTriangles.Length / 3)
                throw new IndexOutOfRangeException("triangleIndex");

            triangleIndex /= 3;

            return new Vector2D[] { uvs[vertexTriangles[triangleIndex - 1]], uvs[vertexTriangles[triangleIndex]], uvs[vertexTriangles[triangleIndex + 1]] };
        }

    }
}