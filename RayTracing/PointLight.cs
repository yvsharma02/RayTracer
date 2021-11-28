namespace RayTracing
{
    public class PointLight : LightSource
    {
        public readonly Vector3D Location;
        public readonly RTColor Color;

        public PointLight(RTColor color, Vector3D location)
        {
            this.Color = color;
            this.Location = location;
        }
    }
}
