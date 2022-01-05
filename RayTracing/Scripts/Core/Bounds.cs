namespace RayTracing
{
    public struct Bounds
    {
        public const int BoundaryPointCount = 8;

        public readonly Vector3D LowerBounds;
        public readonly Vector3D UpperBounds;

        public Bounds(Vector3D lower, Vector3D upper)
        {
            this.LowerBounds = lower;
            this.UpperBounds = upper;
        }

        public Vector3D GetBoundaryPoint(int index)
        {
            Vector3D diff = UpperBounds - LowerBounds;

            switch (index)
            {
                case 0: return LowerBounds;
                case 1: return LowerBounds + new Vector3D(diff.x, 0, 0);
                case 2: return LowerBounds + new Vector3D(0, diff.y, 0);
                case 3: return LowerBounds + new Vector3D(0, 0, diff.z);
                case 4: return LowerBounds + new Vector3D(diff.x, diff.y, 0);
                case 5: return LowerBounds + new Vector3D(diff.x, 0, diff.z);
                case 6: return LowerBounds + new Vector3D(0, diff.y, diff.z);
                case 7: return UpperBounds;
                default: throw new IndexOutOfRangeException();
            }

        }

        public bool IsInside(Vector3D point)
        {
            return RTMath.LiesInsideBounds(point, LowerBounds, UpperBounds);
        }

        public Bounds RegenerateContainingPoint(Vector3D point)
        {
            float[] lower = new float[3] { LowerBounds[0], LowerBounds[1], LowerBounds[2] };
            float[] upper = new float[3] { UpperBounds[0], UpperBounds[1], UpperBounds[2] };

            for (int i = 0; i < 3; i++)
            {
                if (point[i] < lower[i])
                    lower[i] = point[i] - Vector3D.EPSILON;
                else if (point[i] > upper[i])
                    upper[i] = lower[i] + Vector3D.EPSILON;

            }

            return new Bounds(new Vector3D(lower[0], lower[1], lower[2]), new Vector3D(upper[0], upper[1], upper[2]));
        }

        public Bounds RegenrateContaingBounds(Bounds bounds)
        {
            Bounds resultantBound = this;

            for (int i = 0; i < BoundaryPointCount; i++)
            {
                Vector3D pt = bounds.GetBoundaryPoint(i);

                if (!resultantBound.IsInside(pt))
                    resultantBound = resultantBound.RegenerateContainingPoint(pt);
            }

            return resultantBound;
        }
    }
}