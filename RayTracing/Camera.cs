namespace RayTracing
{
    public class Camera
    {

        private Int2D resolution;

        private Vector3D xAxisLine;
        private Vector3D yAxisLine;
        private Vector3D axisIntersectionPoint;

        private Vector3D eyePosition;
        private float xAxisSize;
        private float yAxisSize;
        
        // In Radians.
        private float rotation;

        private Int2D raysPerPixel;

        private Vector3D cameraForwardDirection;
        private Vector3D footOfPerpendiuclarFromEye;

        private Vector3D cam_topLeft;
        private Vector3D cam_topRight;
        private Vector3D cam_buttomRight;
        private Vector3D cam_buttomLeft;

        private int bounceLimit;
        public int BounceLimit
        {
            get
            {
                return bounceLimit;
            }
        }

        public Int2D RaysPerPixel
        {
            get
            {
                return raysPerPixel;
            }
        }

        public Int2D Resolution
        {
            get
            {
                return resolution;
            }
        }

        public Camera(Vector3D xal, Vector3D yal, Vector3D aip, Vector3D ep, float xas, float yas, float rotation, Int2D res, Int2D rpp, int bounceLimit)
        {
            xal = xal.Normalize();
            yal = yal.Normalize();

            if (!Vector3D.ArePerpendicular(xal, yal))
                throw new ArgumentException("xAxisLine and yAxisLine should be perpendicular");

            this.resolution = res;
            this.xAxisLine = xal;
            this.yAxisLine = yal;
            this.axisIntersectionPoint = aip;
            this.eyePosition = ep;
            this.xAxisSize = xas;
            this.yAxisSize = yas;
            this.rotation = rotation;
            this.raysPerPixel = rpp;
            this.bounceLimit = bounceLimit;

            Initialise();
        }

        private void Initialise()
        {
            rotation = (float) (rotation % (2 * Math.PI));

            Vector3D n = Vector3D.Cross(xAxisLine, yAxisLine).Normalize();
            Vector3D e = eyePosition;
            Vector3D o = axisIntersectionPoint;

            float lamda = Vector3D.Dot(o - e, n);

            footOfPerpendiuclarFromEye = e + n * lamda;
            cameraForwardDirection = (n * lamda * -1f).Normalize();

            Vector3D finalXAxis = (xAxisLine * Math.Cos(rotation) + yAxisLine * Math.Sin(rotation));
            Vector3D finalYAxis = (yAxisLine * Math.Cos(rotation) - xAxisLine * Math.Sin(rotation));

            xAxisLine = finalXAxis.Normalize();
            yAxisLine = finalYAxis.Normalize();

            cam_topLeft = footOfPerpendiuclarFromEye - (xAxisLine * (xAxisSize / 2f)) + (yAxisLine * (yAxisSize / 2f));
            cam_topRight = footOfPerpendiuclarFromEye + (xAxisLine * (xAxisSize / 2f)) + (yAxisLine * (yAxisSize / 2f));
            cam_buttomRight = footOfPerpendiuclarFromEye + (xAxisLine * (xAxisSize / 2f)) - (yAxisLine * (yAxisSize / 2f));
            cam_buttomLeft = footOfPerpendiuclarFromEye - (xAxisLine * (xAxisSize / 2f)) - (yAxisLine * (yAxisSize / 2f));
        }

        public Ray PixelIndexToRay(Int2D pixel, Int2D pixelRayIndex)
        {
            Vector3D origin = eyePosition;

            float percentX = (pixel.x * raysPerPixel.x + pixelRayIndex.x) / (float) ((resolution.x + 1) * raysPerPixel.x);
            float percentY = (pixel.y * raysPerPixel.y + pixelRayIndex.y) / (float)((resolution.y + 1) * raysPerPixel.y);

            Vector3D screenPt = cam_topLeft + (cam_topRight - cam_topLeft) * percentX + (cam_buttomLeft - cam_topLeft) * percentY;
            Vector3D dir = screenPt - origin;

            return new Ray(origin, dir);
        }
    }
}
