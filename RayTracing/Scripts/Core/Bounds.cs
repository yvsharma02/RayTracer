namespace RayTracing
{
    public struct Bounds
    {
        public static readonly Bounds Zero = new Bounds(Vector3D.Zero, Vector3D.Zero);

        public const int BoundaryPointCount = 8;

        public readonly Vector3D LowerBounds;
        public readonly Vector3D UpperBounds;

        public Vector3D Center => (UpperBounds - LowerBounds) / 2f;

        public Bounds(Vector3D lower, Vector3D upper)
        {
            this.LowerBounds = lower;
            this.UpperBounds = upper;
        }

        public Bounds(float[] lower, float[] upper)
        {
            if (lower.Length != 3 || upper.Length != 3)
                throw new ArgumentException("lower and upper must be a 3D Vector.");

            this.LowerBounds = new Vector3D(lower[0], lower[1], lower[2]);
            this.UpperBounds = new Vector3D(upper[0], upper[1], upper[2]);
        }

        public static Bounds GenerateNewContainingPoints(params Vector3D[] points)
        {
            if (points.Length == 0)
                return default;

            float[] lower = new float[] { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity };
            float[] upper = new float[] { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity };

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (points[i][j] < lower[j])
                        lower[j] = points[i][j] - Vector3D.EPSILON;
                    if (points[i][j] > upper[j])
                        upper[j] = points[i][j] + Vector3D.EPSILON;
                }
            }

            return new Bounds(lower, upper);
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

        public bool ContainsPoint(Vector3D point)
        {
            return RTMath.LiesInsideBounds(point, LowerBounds, UpperBounds);
        }

        public bool ContainsBoundPartially(Bounds otherBound)
        {
            for (int i = 0; i < BoundaryPointCount; i++)
                if (this.ContainsPoint(otherBound.GetBoundaryPoint(i)))
                    return true;

            return false;
        }

        public bool ContainsBoundCompletely(Bounds otherBound)
        {
            for (int i = 0; i < BoundaryPointCount; i++)
                if (!this.ContainsPoint(otherBound.GetBoundaryPoint(i)))
                    return false;

            return true;
        }

        public Bounds RegenerateContainingPoints(params Vector3D[] points)
        {
            float[] lower = new float[3] { LowerBounds[0], LowerBounds[1], LowerBounds[2] };
            float[] upper = new float[3] { UpperBounds[0], UpperBounds[1], UpperBounds[2] };

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (points[i][j] < lower[j])
                        lower[j] = points[i][j] - Vector3D.EPSILON;
                    if (points[i][j] > upper[j])
                        upper[j] = points[i][j] + Vector3D.EPSILON;

                }
            }
            return new Bounds(lower, upper);
        }

        public Bounds RegenrateContaingBounds(Bounds bounds)
        {
            Bounds resultantBound = this;

            for (int i = 0; i < BoundaryPointCount; i++)
            {
                Vector3D pt = bounds.GetBoundaryPoint(i);

                if (!resultantBound.ContainsPoint(pt))
                    resultantBound = resultantBound.RegenerateContainingPoints(pt);
            }

            return resultantBound;
        }

        public Bounds[] SubdivideIntoOctants()
        {
            Bounds[] octants = new Bounds[8];
            Vector3D mid = Center;

            octants[0] = new Bounds(mid, UpperBounds);
            octants[1] = new Bounds(new Vector3D(LowerBounds.x, mid.y, mid.z), new Vector3D(mid.x, UpperBounds.y, UpperBounds.z));
            octants[2] = new Bounds(new Vector3D(LowerBounds.x, LowerBounds.y, mid.z), new Vector3D(mid.x, mid.y, UpperBounds.z));
            octants[3] = new Bounds(new Vector3D(mid.x, LowerBounds.y, mid.z), new Vector3D(UpperBounds.x, mid.y, UpperBounds.z));

            octants[4] = new Bounds(new Vector3D(mid.x, mid.y, LowerBounds.z), new Vector3D(UpperBounds.x, UpperBounds.y, mid.z));
            octants[5] = new Bounds(new Vector3D(LowerBounds.x, mid.y, LowerBounds.z), new Vector3D(mid.x, UpperBounds.y, mid.z));
            octants[6] = new Bounds(new Vector3D(LowerBounds.x, LowerBounds.y, LowerBounds.z), new Vector3D(mid.x, mid.y, mid.z));
            octants[7] = new Bounds(new Vector3D(mid.x, LowerBounds.y, LowerBounds.z), new Vector3D(UpperBounds.x, mid.y, mid.z));

            return octants;
        }

        public override string ToString()
        {
            return String.Format("[from {0} to {1}]", LowerBounds, UpperBounds);
        }
    }
}