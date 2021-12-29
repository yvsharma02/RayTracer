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

        public Mesh Build(Transfomration transform, ShapeShader shader)
        {
            return new Mesh(transform, shader, vertices, vertexTriangles, normals, normalTriangles, uvs, uvTriangles);
        }
    }
}
