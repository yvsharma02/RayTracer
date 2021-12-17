namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        public ShapeShader Shader { get; private set; }
        public override bool IsLightSource => false;

        public Shape(Vector3D position, ShapeShader shader) : base(position)
        {
            this.Shader = shader;
        }

        public virtual Int2D POCToTexturePixelIndex(Shape subShape, Vector3D pointOfContact, Int2D TextureDimensions)
        {
            throw new NotImplementedException();
        }

        public abstract Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact);

    }
}