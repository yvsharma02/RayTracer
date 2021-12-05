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

            float distance = poc.Magnitude();

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
            float C = (o - p).MagnitudeSq() - sphereRadius * sphereRadius;

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

        public static bool PointInPlane(Vector3D pointToCheck, Vector3D planeNormal, Vector3D pointInPlane)
        {
            planeNormal = planeNormal.Normalize();
            float d = Vector3D.Dot(planeNormal, pointInPlane);

            return Math.Abs(Vector3D.Dot(pointToCheck, planeNormal) - d) <= Vector3D.EPSILON;
        }

        public static Vector3D? RayPlaneContact(Ray ray, Vector3D firstAxis, Vector3D secondAxis, Vector3D planeOrigin)
        {
            if (!Vector3D.ArePerpendicular(firstAxis, secondAxis))
                throw new ArgumentException("First and Second axis should be perpendicular.");

            Vector3D o = ray.Origin;
            Vector3D p = planeOrigin;
            Vector3D n = Vector3D.Cross(firstAxis, secondAxis).Normalize();
            Vector3D d = ray.Direction;

            float lamda = Vector3D.Dot(p - o, n) / Vector3D.Dot(d, n);

            if (lamda >= 0)
            {
                Vector3D poc = o + (d * lamda);

                Vector3D r = poc - planeOrigin;

                float firstAxisProjectionLength = Vector3D.Dot(r, firstAxis) / firstAxis.Magnitude();
                float secondAxisProjectionLength = Vector3D.Dot(r, secondAxis) / secondAxis.Magnitude();

                if (firstAxisProjectionLength < 0 || secondAxisProjectionLength < 0)
                    return null;

                if (firstAxisProjectionLength <= firstAxis.Magnitude() && secondAxisProjectionLength <= secondAxis.Magnitude())
                    return poc;

                return null;

            }
            else
                return null;
        }
    }
}
