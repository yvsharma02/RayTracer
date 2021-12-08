namespace RayTracing
{
    public static class MathUtil
    {
        public static bool LiesOnLine(Vector3D point, Vector3D lineOrigin, Vector3D lineDir)
        {
            return Vector3D.Cross((point - lineOrigin).Normalize(), lineDir.Normalize()).Magnitude() < Vector3D.EPSILON;
        }

        public static float[] SolveLinear(float a1, float b1, float c1, float a2, float b2, float c2)
        {
            return new float[] { (c1 * b2 - b2 * c1) / (a1 * b2 - b2 * a1), (c1 * a2 - c2 * a1) / (b1 * a2 - b2 * a1) };
        }

        public static bool LiesOnLineInBetweenPoints(Vector3D point, Vector3D lineStart, Vector3D lineEnd)
        {
            Vector3D dir = lineEnd - lineStart;

            if (!LiesOnLine(point, lineStart, dir))
                return false;

            float param = Vector3D.Dot((point - lineStart), dir) / dir.MagnitudeSq();
            float paramLimit = Vector3D.Dot(lineEnd - lineStart, dir) / dir.MagnitudeSq();

            if (param * paramLimit < 0)
                return false;

            return Math.Abs(param) < Math.Abs(paramLimit);
        }

        public static Vector3D? LineLineIntersectionPoint(Vector3D l1Origin, Vector3D l1Dir, Vector3D l2Origin, Vector3D l2Dir)
        {
            if (ShortestDistBetweenLines(l1Origin, l1Dir, l2Origin, l2Dir) > Vector3D.EPSILON)
                return null;

            Vector3D a1 = l1Origin;
            Vector3D p = l1Dir;

            Vector3D a2 = l2Origin;
            Vector3D q = l2Dir;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                        continue;

                    float di = a2[i] - a1[i];
                    float dj = a2[j] - a1[j];

                    float divider = p[i] * q[j] - p[j] * q[i];

                    if (Math.Abs(divider) < Vector3D.EPSILON)
                        continue;

                    float u = ((di * p[j]) - (dj * p[i])) / divider;

                    return a2 + (q * u);
                }
            }

            return null;
        }

        public static float ShortestDistBetweenLines(Vector3D l1Origin, Vector3D l1Dir, Vector3D l2Origin, Vector3D l2Dir)
        {
            Vector3D cross = Vector3D.Cross(l1Dir, l2Dir);

            float dist = Math.Abs((Vector3D.Dot((l2Origin - l1Origin), cross) / cross.Magnitude()));

            if (dist < Vector3D.EPSILON)
                return 0f;

            return dist;
        }

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

        public static Vector3D? RayPlaneContact(Ray ray, Vector3D firstAxis, Vector3D secondAxis, Vector3D planeOrigin, bool checkBounds)
        {
            if (checkBounds && !Vector3D.ArePerpendicular(firstAxis, secondAxis))
                throw new ArgumentException("First and Second axis should be perpendicular.");

            Vector3D o = ray.Origin;
            Vector3D p = planeOrigin;
            Vector3D n = Vector3D.Cross(firstAxis, secondAxis).Normalize();
            Vector3D d = ray.Direction;

            float lamda = Vector3D.Dot(p - o, n) / Vector3D.Dot(d, n);

            if (lamda >= 0)
            {
                Vector3D poc = o + (d * lamda);

                if (!checkBounds)
                    return poc;

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
