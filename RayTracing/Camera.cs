namespace RayTracing
{
    public class Camera
    {
        public readonly Vector3D eyePosition;

        public readonly Vector3D screenTopRight;
        public readonly Vector3D screenTopLeft;
        public readonly Vector3D screenButtomLeft;
        public readonly Vector3D screenButtomRight;

        public readonly int ResolutionX;
        public readonly int ResolutionY;


        public readonly int RaysPerPixelX;
        public readonly int RaysPerPixelY;

        public Camera(Vector3D eyePosition, Vector3D sTR, Vector3D sTL, Vector3D sBL, Vector3D sBR,int rX, int rY, int rppx = 0, int rppY = 0)
        {
            this.eyePosition = eyePosition;

            this.screenTopRight = sTR;
            this.screenTopLeft = sTL;
            this.screenButtomLeft = sBL;
            this.screenButtomRight = sBR;
            this.ResolutionX = rX;
            this.ResolutionY = rY;

            this.RaysPerPixelX = 0;
            this.RaysPerPixelY = 0;
        }
    }
}
