namespace RayTracing
{
    public class CubeShape : Shape
    {
        private PlaneShape[] faces;

        public CubeShape(Vector3D firstAxis, Vector3D secondAxis, Vector3D thirdAxis, Vector3D axisIntersection, ShapeShader shader) : base(axisIntersection + (firstAxis + secondAxis + thirdAxis) / 2f, shader)
        {
            if (!Vector3D.ArePerpendicular(firstAxis, secondAxis) || !Vector3D.ArePerpendicular(firstAxis, thirdAxis) || !Vector3D.ArePerpendicular(secondAxis, thirdAxis))
                throw new ArgumentException("Axis should be perpendicular");

            faces = new PlaneShape[6];

            int c = 0;

            faces[c++] = new PlaneShape(axisIntersection, secondAxis, firstAxis, shader);
            faces[c++] = new PlaneShape(axisIntersection, thirdAxis, secondAxis, shader);
            faces[c++] = new PlaneShape(axisIntersection, firstAxis, thirdAxis, shader);
            
            Vector3D oppositeAxisIntersection = axisIntersection + (firstAxis + secondAxis + thirdAxis);

            faces[c++] = new PlaneShape(oppositeAxisIntersection, -firstAxis, -secondAxis, shader);
            faces[c++] = new PlaneShape(oppositeAxisIntersection, -secondAxis, -thirdAxis, shader);
            faces[c++] = new PlaneShape(oppositeAxisIntersection, -thirdAxis, -firstAxis, shader);
        }
        protected override Vector3D? CalculateRayContactPosition(Ray ray, out WorldObject subshape)
        {
            float closestDist = float.PositiveInfinity;
            Vector3D? closestPt = null;
            WorldObject closestSubshape = null;

            for (int i = 0; i < faces.Length; i++)
            {
                WorldObject currentHitSubshape;
                Vector3D poc;
                if (faces[i].HitsRay(ray, out poc, out currentHitSubshape))
                {
                    float dist = Vector3D.Distance(poc, ray.Origin);
                    if (dist < closestDist)
                    {
                        closestSubshape = faces[i];
                        closestPt = poc;
                        closestDist = dist;
                    }
                }
            }

            subshape = closestSubshape;
            return closestPt;
        }
        public override Vector3D CalculateNormal(Shape shape, Vector3D pointOfContact)
        {
            return shape.CalculateNormal(null, pointOfContact);
        }
    }
}
