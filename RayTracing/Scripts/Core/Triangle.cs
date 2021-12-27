namespace RayTracing
{
    public struct Triangle
    {
        public readonly Vector3D Point0;
        public readonly Vector3D Point1;
        public readonly Vector3D Point2;

        public Vector3D Origin => Point0;
        public Vector3D Side0 => Point1 - Point0;
        public Vector3D Side1 => Point2 - Point0;
        public Vector3D Centroid => (Point0 + Point1 + Point2) / 3f;
        public Vector3D AreaVector => Vector3D.Cross(Side0, Side1) * 0.5f;
        public Vector3D Normal => AreaVector.Normalize();
        public float Area => AreaVector.Magnitude();

        public Triangle(Vector3D p0, Vector3D p1, Vector3D p2)
        {
            this.Point0 = p0;
            this.Point1 = p1;
            this.Point2 = p2;
        }

        public Vector3D this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Point0;
                    case 1: return Point1;
                    case 2: return Point2;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public Vector3D CalculateBarycentricPoint(Vector3D point)
        {
            return RTMath.CalculateBaycentricCoords(this, point);
        }
    }
}
