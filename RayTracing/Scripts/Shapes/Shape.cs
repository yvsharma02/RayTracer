namespace RayTracing
{
    public abstract class Shape : WorldObject
    {
        public override int TypeID => (int) TypeIDs.Shape;

        public ShapeShader Shader { get; protected set; }

        public Shape(Transfomration transform, ShapeShader shader) : base(transform)
        {
            this.Shader = shader;
        }

        public abstract Vector3D CalculateNormal(Vector3D pointOfContact);

        protected abstract Vector3D? CalculateRayContactPosition(Ray ray, out Shape subShape);

        public abstract Vector2D CalculateUV(Vector3D pointOfContact);

        public virtual bool HitsRay(Ray ray)
        {
            return HitsRay(ray, out _, out Shape _);
        }

        public virtual bool HitsRay(Ray ray, out Vector3D pointOfContact, out Shape subObject)
        {
            Vector3D? poc = CalculateRayContactPosition(ray, out subObject);

            if (poc.HasValue)
            {
                pointOfContact = poc.Value;
                return true;
            }
            else
            {
                pointOfContact = new Vector3D(0f, 0f, 0f);
                return false;
            }
        }
    }
}