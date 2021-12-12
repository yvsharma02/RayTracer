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

            faces[c++] = new PlaneShape(axisIntersection, secondAxis, firstAxis, null);
            faces[c++] = new PlaneShape(axisIntersection, thirdAxis, secondAxis, null);
            faces[c++] = new PlaneShape(axisIntersection, firstAxis, thirdAxis, null);
            
            Vector3D oppositeAxisIntersection = axisIntersection + (firstAxis + secondAxis + thirdAxis);

            faces[c++] = new PlaneShape(oppositeAxisIntersection, -firstAxis, -secondAxis, null);
            faces[c++] = new PlaneShape(oppositeAxisIntersection, -secondAxis, -thirdAxis, null);
            faces[c++] = new PlaneShape(oppositeAxisIntersection, -thirdAxis, -firstAxis, null);
        }
        protected override Vector3D? CalculateRayContactPosition(Ray ray)
        {
            float closestDist = float.PositiveInfinity;
            Vector3D? closestPt = null;

            for (int i = 0; i < faces.Length; i++)
            {
                Vector3D poc;
                if (faces[i].HitsRay(ray, out poc))
                {
                    float dist = Vector3D.Distance(poc, ray.Origin);
                    if (dist < closestDist)
                    { 
                        closestPt = poc;
                        closestDist = dist;
                    }
                }
            }

            return closestPt;
        }
        public override Vector3D CalculateNormal(Vector3D pointOfContact)
        {
            for (int i = 0; i < faces.Length; i++)
                if (RTMath.PointInPlane(pointOfContact, faces[i].CalculateNormal(pointOfContact), faces[i].Position))
                    return faces[i].CalculateNormal(pointOfContact);

            return new Vector3D();
        }
    }
}
