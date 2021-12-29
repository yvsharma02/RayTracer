namespace RayTracing
{
    public static class RTMath
    {
        public static bool LiesInsideTriangle(Vector3D point, bool checkIfPointLiesInPlane, Triangle triangle)
        {
            Vector3D p = point;

            Vector3D p0 = triangle.Point0 - p;
            Vector3D p1 = triangle.Point1 - p;
            Vector3D p2 = triangle.Point2 - p;

            p -= p;

            Vector3D u = Vector3D.Cross(p1, p2);
            Vector3D v = Vector3D.Cross(p2, p0);
            Vector3D w = Vector3D.Cross(p0, p1);

            if (Vector3D.Dot(u, v) <= 0f)
                return false;
            if (Vector3D.Dot(u, w) <= 0f)
                return false;
            if (Vector3D.Dot(v, w) <= 0f)
                return false;

            return true;
        }

        public static float[,] InvertMatrix(float[,] sqMatrix)
        {
            if (sqMatrix == null)
                throw new ArgumentNullException();

            if (sqMatrix.GetLength(0) != sqMatrix.GetLength(1))
                throw new ArgumentException("Matrix should be square.");

            int len = sqMatrix.GetLength(0);

            float det = Determinant(sqMatrix);

            float[,] inverted = new float[len, len];

            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                    inverted[i, j] = Cofactor(sqMatrix, i, j) / det;

            return inverted;
        }

        public static float[,] MultiplyMatrix(float[,] A, float[,] B)
        {
            if (A == null || B == null)
                throw new ArgumentNullException();

            if (A.GetLength(1) != B.GetLength(0))
                throw new ArgumentException("Dimension Mismatch");

            int resDimensions = A.GetLength(1);
            float[,] res = new float[A.GetLength(0), B.GetLength(1)];

            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    for (int k = 0; k < resDimensions; k++)
                        res[i, j] += A[i, k] * B[k, j];
                }
            }

            return res;
        }

        public static float Cofactor(float[,] matrix, int x, int y)
        {
            return Minor(matrix, x, y) * ((x + y) % 2 == 0 ? 1 : -1);
        }

        public static float Minor(float[,] matrix, int columToSkip, int rowToSkip)
        {
            if (matrix == null)
                throw new ArgumentNullException();

            if (matrix.GetLength(0) != matrix.GetLength(1))
                throw new ArgumentException("Dimension Mismatch");

            int len = matrix.GetLength(0);

            if (columToSkip < 0 || columToSkip >= len)
                throw new ArgumentOutOfRangeException("columToSkip");

            if (rowToSkip < 0 || rowToSkip >= len)
                throw new ArgumentOutOfRangeException("rowToSkip");

            float[,] subMatrix = SubMatrix(matrix, columToSkip, rowToSkip);
            return Determinant(subMatrix);
        }

        public static float[,] SubMatrix(float[,] matrix, int columToSkip, int rowToSkip)
        {
            int len = matrix.GetLength(0);

            float[,] subMat = new float[len - 1, len - 1];

            int x = 0;

            for (int i = 0; i < len; i++)
            {
                if (i == columToSkip)
                    continue;

                int y = 0;

                for (int j = 0; j < len; j++)
                {
                    if (j == rowToSkip)
                        continue;

                    subMat[x, y++] = matrix[i, j];
                }
                x++;
            }

            return subMat;
        }

        public static float Determinant(float[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1))
                throw new ArgumentException("Matrix should be square.");

            if (matrix.GetLength(0) == 1)
                return matrix[0, 0];

            float res = 0f;

            for (int i = 0; i < matrix.GetLength(0); i++)
                res += matrix[i, 0] * Cofactor(matrix, i, 0);

            return res;
        }

        public static float LinePointDistance(Vector3D lineOrigin, Vector3D lineDir, Vector3D point)
        {
            lineDir = lineDir.Normalize();

            float param = Vector3D.Dot(point - lineOrigin, lineDir);
            Vector3D foorPerpendicular = lineOrigin + (lineDir * param);

            return Vector3D.Distance(foorPerpendicular, lineOrigin);
        }

        public static Vector3D CalculateReflectedRayDirection(Vector3D incidentRay, Vector3D normal)
        {
            normal = normal.Normalize();
            incidentRay = incidentRay.Normalize();

            return incidentRay - normal * 2f * Vector3D.Dot(incidentRay, normal);
        }

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

        public static Vector3D CalculateBaycentricCoords(Triangle triangle, Vector3D poc)
        {
            float mainTriangleArea = triangle.Area;

            Triangle[] subtriangles = new Triangle[]
            {
                new Triangle(triangle[0], triangle[1], poc), new Triangle(triangle[1], triangle[2], poc), new Triangle(triangle[2], triangle[0], poc)
            };

            // ith vertex will get the area ratio of the subtriangle opposite to the ith triangle.
            Vector3D areaRatios = new Vector3D(subtriangles[1].Area, subtriangles[2].Area, subtriangles[0].Area) / mainTriangleArea;

            return areaRatios;
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

        public static Vector3D? RayTriangleIntersectionPoint(Ray ray, Triangle triangle)
        {

            Vector3D? poc = RTMath.RayPlaneContact(ray, triangle);

            if (!poc.HasValue)
                return null;

            return RTMath.LiesInsideTriangle(poc.Value, false, triangle) ? poc.Value : null;
        }

        public static bool LiesInsideBounds(Vector3D point, Vector3D lowerBounds, Vector3D upperBounds)
        {
            for (int i = 0; i < 3; i++)
            {
                if (point[i] > upperBounds[i] || point[i] < lowerBounds[i])
                    return false;
            }

            return true;
        }

        public static Vector3D? RayBoundsContact(Ray ray, Vector3D lower, Vector3D upper)
        {
            Vector3D? closestPOC = null;
            float closestDistSq = float.MaxValue;

            Vector3D diff = upper - lower;

            Vector3D[] axes = new Vector3D[3];

            for (int i = 0; i < 2; i++)
            {
                axes[0] = new Vector3D((i == 0 ? 1 : -1) * diff.x, 0, 0);
                axes[1] = new Vector3D(0, (i == 0 ? 1 : -1) * diff.y, 0);
                axes[2] = new Vector3D(0, 0, (i == 0 ? 1 : -1) * diff.z);

                for (int j = 0; j < 3; j++)
                {
                    Vector3D[] consideredAxes = new Vector3D[2];
                    int c = 0;

                    for (int k = 0; k < 3; k++)
                    {
                        if (j == k)
                            continue;

                        consideredAxes[c++] = axes[k];
                    }

                    Vector3D? poc = RayBoundedPlaneContact(ray, i == 0 ? lower : upper, consideredAxes[0], consideredAxes[1]);
                    if (poc.HasValue)
                    {
                        float distSq = Vector3D.DistanceSq(ray.Origin, poc.Value);
                        if (distSq < closestDistSq)
                        {
                            closestPOC = poc;
                            closestDistSq = distSq;
                        }
                    }
                }
            }

            return closestPOC;
        }

        public static bool RayHitsTriangle(Ray ray, Triangle triangle)
        {
            return RayTriangleIntersectionPoint(ray, triangle).HasValue;
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

        public static Vector3D? RayBoundedPlaneContact(Ray ray, Vector3D planeOrigin, Vector3D firstPerpAxis, Vector3D secondPerpAxis)
        {
            if (Math.Abs(Vector3D.Dot(firstPerpAxis, secondPerpAxis)) >= Vector3D.EPSILON)
                throw new ArgumentException("Axis should be perpendicular");

            Vector3D? poc = RayPlaneContact(ray, planeOrigin, planeOrigin + firstPerpAxis, planeOrigin + secondPerpAxis);

            if (poc.HasValue)
            {
                Vector3D r = poc.Value - planeOrigin;

                float firstAxisProjectionLength = Vector3D.Dot(r, firstPerpAxis) / firstPerpAxis.Magnitude();
                float secondAxisProjectionLength = Vector3D.Dot(r, secondPerpAxis) / secondPerpAxis.Magnitude();

                if (firstAxisProjectionLength < 0 || secondAxisProjectionLength < 0)
                    return null;

                if (firstAxisProjectionLength <= firstPerpAxis.Magnitude() && secondAxisProjectionLength <= secondPerpAxis.Magnitude())
                    return poc;

                return null;
            }

            return null;
        }

        public static Vector3D? RayBoundedPlaneContact(Ray ray, Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
        {
            if (!LiesOnPlane(p0, p1, p2, p3))
                throw new ArgumentException("Points are not coplanar");

            bool firstTriangle = true;

            Vector3D? poc = RayPlaneContact(ray, p0, p1, p2);

            if (poc == null)
            {
                poc = RayPlaneContact(ray, p1, p3, p2);
                firstTriangle = false;
            }

            if (poc != null)
            {
                if (firstTriangle)
                    return LiesInsideTriangle(poc.Value, false, new Triangle(p0, p1, p2)) ? poc : null;
                else
                    return LiesInsideTriangle(poc.Value, false, new Triangle(p1, p3, p2)) ? poc : null;
            }

            return null;
        }

        public static bool LiesOnPlane(Triangle points, Vector3D point)
        {
            Vector3D normal = points.Normal;
            float d = Vector3D.Dot(points.Origin, normal);

            return MathF.Abs(Vector3D.Dot(point, normal) - d) <= Vector3D.EPSILON;
        }

        public static bool LiesOnPlane(Vector3D pt1, Vector3D pt2, Vector3D pt3, Vector3D point)
        {
            return LiesOnPlane(new Triangle(pt1, pt2, pt3), point);
        }

        public static Vector3D? RayPlaneContact(Ray ray, Vector3D pt1, Vector3D pt2, Vector3D pt3)
        {
            return RayPlaneContact(ray, new Triangle(pt1, pt2, pt3));
        }

        public static Vector3D? RayPlaneContact(Ray ray, Triangle triangle)
        {
            Vector3D o = ray.Origin;
            Vector3D p = triangle.Origin;
            Vector3D n = triangle.Normal;
            Vector3D d = ray.Direction;

            float lamda = Vector3D.Dot(p - o, n) / Vector3D.Dot(d, n);

            if (lamda >= 0)
            {
                Vector3D poc = o + (d * lamda);
                return poc;
            }
            else
                return null;
        }
    }
}