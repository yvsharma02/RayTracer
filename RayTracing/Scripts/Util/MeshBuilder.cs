namespace RayTracing
{
    public class MeshBuilder
    {
        public Vector3D[] vertices;
        public Vector2D[] uvs;
        public int[] vertexTriangles;
        public int[] uvTriangles;

        public Mesh Build(Vector3D position, ShapeShader shader, float scale = 1f)
        {
            Vector3D[] positions = new Vector3D[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                positions[i] = (vertices[i]) * scale + position;

            return new Mesh(position, shader, positions, vertexTriangles, uvs, uvTriangles);
        }
    }
}
