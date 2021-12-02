namespace RayTracing
{
    public static class MathUtil
    {
        public static bool RayHitsSphere(Ray ray, Vector3D sphereCenter, float sphereRadius, out Vector3D pointOfContact)
        {
            Vector3D? poc = RaySpherePointOfContact(ray, sphereCenter, sphereRadius);

            if (poc.HasValue)
            {
                pointOfContact = poc.Value;
                return true;
            }

            pointOfContact = new Vector3D();
            return false;
        }

        public static bool RayHitsSphere(Ray ray, Vector3D sphereCenter, float sphereRadius)
        {
            Vector3D o = ray.Origin;
            Vector3D d = ray.Direction;
            Vector3D p = sphereCenter;

            float lambda = Vector3D.Dot((o - p), d);

            Vector3D poc = (o - p) + d * lambda;

            float distance = poc.Magnitute();

            if (distance <= sphereRadius && lambda >= 0f)
                return true;

            return false;
        }

        public static Vector3D? RaySpherePointOfContact(Ray ray, Vector3D sphereCenter, float sphereRadius)
        {
            Vector3D o = ray.Origin;
            Vector3D d = ray.Direction;
            Vector3D p = sphereCenter;

            float B = 2f * Vector3D.Dot(d, (o - p));
            float C = 2f * (o - p).MagnitudeSq() - sphereRadius * sphereRadius;

            float D = B * B - 4 * C;

            if (D < 0)
                return null;

            float r1 = (float)((-B + Math.Sqrt(D)) / 2f);
            float r2 = (float)((-B - Math.Sqrt(D)) / 2f);

            float realRoot = 0f;

            if (r1 < 0 && r2 < 0)
                return null;
            else if (r1 > 0 && r2 > 0)
                realRoot = r1 < r2 ? r1 : r2;
            else
                realRoot = r1 > 0 ? r1 : r2;

            return o + (d * realRoot);
        }
    }
}
