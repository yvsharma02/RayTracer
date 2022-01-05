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

        public Mesh Build(Transfomration transform, ShapeShader shader, bool includeNormals)
        {
            return new Mesh(transform, shader, vertices, vertexTriangles, includeNormals ? normals : null, includeNormals ? normalTriangles : null, uvs, uvTriangles);
        }
    }
}
