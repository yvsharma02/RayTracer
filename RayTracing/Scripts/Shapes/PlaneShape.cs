namespace RayTracing
{
    public class PlaneShape : Shape
    {
        private Vector3D normalVector;

        private Vector3D firstAxis;
        private Vector3D secondAxis;

        // Size extends in both positive and negative direction.
        public PlaneShape(Vector3D axisIntersection, Vector3D firstAxisSize, Vector3D secondAxisSize, ShapeShader shader) : base(axisIntersection, shader)
        {
            if (!Vector3D.ArePerpendicular(firstAxisSize, secondAxisSize))
                throw new ArgumentException("Both axis should be perpendicular");

            this.firstAxis = firstAxisSize;
            this.secondAxis = secondAxisSize;

            this.normalVector = Vector3D.Cross(firstAxisSize, secondAxisSize).Normalize();
        }

        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            return normalVector;
        }

        public override Int2D POCToTexturePixelIndex(Shape shape, Vector3D pointOfContact, Int2D textureDimension)
        {
            float f = (RTMath.LinePointDistance(Position, firstAxis, pointOfContact) / secondAxis.Magnitude());

            int x = ((int) ((RTMath.LinePointDistance(Position, firstAxis, pointOfContact - Position) / secondAxis.Magnitude()) * textureDimension.x));
            int y = ((int) ((RTMath.LinePointDistance(Position, secondAxis, pointOfContact - Position) / firstAxis.Magnitude()) * textureDimension.y));

            return new Int2D(x, y);
        }

        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subshape)
        {
            subshape = null;
            return RTMath.RayBoundedPlaneContact(ray, Position, firstAxis, secondAxis);
        }

    }
}