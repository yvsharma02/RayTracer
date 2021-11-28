namespace RayTracing
{
    public abstract class Shape
    {
        public abstract Vector3D? RayContactPosition(Ray ray);

        public abstract RTColor ReEmit(RTColor incidentColor, Ray incidentRay);

        public bool HitsRay(Ray ray)
        {
            return RayContactPosition(ray).HasValue;
        }

        public bool HitsRay(Ray ray, out Vector3D pointOfContact)
        {
            Vector3D? poc = RayContactPosition(ray);

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
