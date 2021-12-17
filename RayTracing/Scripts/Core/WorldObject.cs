namespace RayTracing
{
    public abstract class WorldObject
    {
        private Vector3D position;

        public Vector3D Position { get => position; }

        public WorldObject(Vector3D position)
        {
            this.position = position;
        }

        public abstract bool IsLightSource { get; }

        protected abstract Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subObject);

        public virtual bool HitsRay(Ray ray)
        {
            return HitsRay(ray, out _, out WorldObject _);
        }

        public virtual bool HitsRay(Ray ray, out Vector3D pointOfContact, out WorldObject subObject)
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
