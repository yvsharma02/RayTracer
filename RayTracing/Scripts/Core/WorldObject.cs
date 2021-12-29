namespace RayTracing
{
    public abstract class WorldObject
    {
        protected Transfomration oldTransform = new Transfomration(Vector3D.Zero, Vector3D.Zero, new Vector3D(1f, 1f, 1f));  
        protected Transfomration transform = new Transfomration(Vector3D.Zero, Vector3D.Zero, new Vector3D(1f, 1f, 1f));

        protected bool newTransformApplied = false;

        public Transfomration Transform
        {
            get
            {
                return transform;
            }
        }

        public void SetTransform(Transfomration newTransform)
        {
            SetTransform(newTransform, true);
        }

        public WorldObject(Transfomration transform)
        {
            // We want to skip calling OnTransformChange here as the child may not have been initialized info needed to process Transform Change yet.
            this.transform = transform;
        }

        protected virtual void SetTransform(Transfomration newTransform, bool transformImmediately)
        {
            this.oldTransform = transform;
            this.transform = newTransform;

            newTransformApplied = false;

            if (transformImmediately)
                ApplyTransform();
        }

        /// <summary>
        /// By default, just sets newTransformApplied to true.
        /// </summary>
        protected virtual void ApplyTransform()
        {
            newTransformApplied = true;
        }

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
