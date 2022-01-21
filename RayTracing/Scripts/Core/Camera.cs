using System.Drawing;

namespace RayTracing
{
    public class Camera : WorldObject
    {
        // Corresponding to Screen X as camera viewport X and screen Y as camera viewport Y
        public static readonly Vector3D DefaultCameraForward = -Vector3D.Forward;

        public static readonly Vector3D DefaultCameraX = new Vector3D(1, 0, 0);
        public static readonly Vector3D DefaultCameraY = new Vector3D(0, 1, 0);

        public override int TypeID => (int) TypeIDs.Camera;

        private Vector3D transformedForward;
        private Vector3D transformedX;
        private Vector3D transformedY;

        public Vector3D TransformedForward => transformedForward;
        public Vector3D TransformedX => transformedX;
        public Vector3D TransformedY => transformedY;
        
        public Vector2D ScreenSize => new Vector2D(transform.Scale.x, transform.Scale.y);
        public Vector2D PixelSize => new Vector2D(ScreenSize.x / Resolution.x, ScreenSize.y / Resolution.y);

        public Vector3D ProjectedScreenSize => ProjectedButtomRight - ProjectedTopLeft;

        public Vector3D ProjectedPixelSize => new Vector3D(ProjectedScreenSize.x / Resolution.x, ProjectedScreenSize.y / Resolution.y);

        public Vector3D EyePosition => transform.Position;

        public Vector3D ProjectedTopLeft            => EyePosition + transformedForward - (transformedX * ScreenSize.x) / 2f + (transformedY * ScreenSize.y) / 2f;
        public Vector3D ProjectedTopRight           => EyePosition + transformedForward + (transformedX * ScreenSize.x) / 2f + (transformedY * ScreenSize.y) / 2f;
        public Vector3D ProjectedButtomLeft         => EyePosition + transformedForward - (transformedX * ScreenSize.x) / 2f - (transformedY * ScreenSize.y) / 2f;
        public Vector3D ProjectedButtomRight        => EyePosition + transformedForward + (transformedX * ScreenSize.x) / 2f - (transformedY * ScreenSize.y) / 2f;

        public readonly RTColor NoHitColor;
        public readonly PixelShader Shader;
        public readonly Int2D Resolution;
        public readonly int BounceLimit;

        public Camera(Transfomration transform, Int2D resolution, PixelShader shader, int bounceLimit, RTColor noHitColor) : base(transform)
        {
            this.NoHitColor = noHitColor;
            this.BounceLimit = bounceLimit;
            this.Shader = shader;
            this.Resolution = resolution;

            ApplyTransform();
        }

        public Vector3D ProjectedPixelCenter(Int2D index)
        {
            if (index.x < 0 || index.y < 0 || index.x > Resolution.x || index.y > Resolution.y)
                throw new IndexOutOfRangeException();

            return ProjectedTopLeft + transformedX * (ScreenSize.x * ((float) index.x /  Resolution.x)) - transformedY * (ScreenSize.y * ((float) index.y / Resolution.y));
        }

        protected override void ApplyTransform()
        {
            transformedForward = transform.Transform(DefaultCameraForward, false, true, false);
            transformedForward *= transform.Scale.z;
            transformedX = transform.Transform(DefaultCameraX, false, true, false);
            transformedY = transform.Transform(DefaultCameraY, false, true, false);

            base.ApplyTransform();
        }
    }
}