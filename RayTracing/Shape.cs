namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        private readonly ShapeShader shader;

        public ShapeShader Shader { get; }
        public override bool IsLightSource => false;

        public Shape(Vector3D position, ShapeShader shader) : base(position)
        {
            this.Shader = shader;
        }

        public virtual Int2D POCToTexturePixelIndex(Vector3D pointOfContact, Int2D TextureDimensions)
        {
            throw new NotImplementedException();
        }

        public abstract Vector3D CalculateNormal(Vector3D pointOfContact);

    }
}