namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        public ShapeShader Shader { get; protected set; }

//        public abstract Bounds Bounds { get; }

        public Shape(Transfomration transform, ShapeShader shader) : base(transform)
        {
            this.Shader = shader;
        }

        public virtual Vector2D CalculateUV(Shape subShape, Vector3D pointOfContact)
        {
            throw new NotImplementedException();
        }

        public abstract Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact);
    }
}