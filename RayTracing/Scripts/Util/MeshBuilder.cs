namespace RayTracing
{
    public class MeshBuilder
    {
        public Vector3D[] vertices;
        public Vector3D[] normals;

        public Vector2D[] uvs;

        public int[] vertexTriangles;
        public int[] normalTriangles;
        public int[] uvTriangles;

        public Mesh Build(Vector3D position, ShapeShader shader, float scale = 1f)
        {
            Vector3D[] transformedVertices = new Vector3D[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                transformedVertices[i] = (vertices[i]) * scale + position;

            return new Mesh(position, shader, transformedVertices, vertexTriangles, normals, normalTriangles, uvs, uvTriangles);
        }
    }
}
