namespace RayTracing
{
    public class TestScene
    {
        private const bool MULTI_THREADING_ENABLED = true;
        private const bool USE_ADVANCE_TRACE = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 16;
        private const int CHUNKS_Y = 16;

        private const int RES_X = 1024;
        private const int RES_Y = 1024;

        private const int RAYS_PER_PIXEL_X = 2;
        private const int RAYS_PER_PIXEL_Y = 2;

        private const int BOUCES = 4;

        private World world;

        private RTColor CalculateColor(RTRay[][] clrs, float iDiv, float rDiv, float gDiv, float bDiv)
        {
            int count = 0;

            float totalIntensity = 0f;

            for (int i = 0; i < clrs.Length; i++)
            {
                count += clrs[i].Length;
                for (int j = 0; j < clrs[i].Length; j++)
                    totalIntensity += clrs[i][j].DestinationColor.Intensity;
            }

            float _i = 0f, r = 0f, g = 0f, b = 0f;

            for (int i = 0; i < clrs.GetLength(0); i++)
            {
                for (int j = 0; j < clrs[i].Length; j++)
                {
                    RTRay ray = clrs[i][j];

                    _i += ray.DestinationColor.Intensity;
                    r += ray.SourceColor.R * (ray.DestinationColor.Intensity / totalIntensity);
                    g += ray.SourceColor.G * (ray.DestinationColor.Intensity / totalIntensity);
                    b += ray.SourceColor.B * (ray.DestinationColor.Intensity / totalIntensity);
                }
            }

            return new RTColor(_i / iDiv, r / rDiv, g / gDiv, b / bDiv);
        }

        public TestScene()
        {
            world = new World(null, null);

            Vector3D PlaneAxis1 = new Vector3D(0, 0, 50);
            Vector3D PlaneAxis2 = new Vector3D(50, 0, 0);

            int lightCountX = 5;
            int lightCountY = 5;

            float lightHeight = 3f;

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1f, 1f),
                new Vector3D(-15, -15, 0),
                new Vector3D(25, 50, -25),
                75,
                75,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES,
                new RTColor(RTColor.MAX_INTENSITY / 3, 123, 123, 123));

            world.SetMainCamera(camera);

            world.AddShape(new PlaneShape(new Vector3D(), PlaneAxis1, PlaneAxis2, (rays, outDir) =>
            {
                return CalculateColor(rays, 1f, 1f, 1f, 1f);
            }));

            RTColor sunColor = new RTColor(RTColor.MAX_INTENSITY, 255, 250, 242);
            Vector3D sunAxis1 = new Vector3D(0.0001f, 0, 0);
            Vector3D sunAxis2 = new Vector3D(0, -0.0001f, 0.0001f);

            world.AddLightSource(new GlobalLight(new Vector3D(0, 100, 0), sunAxis1, sunAxis2, new Int2D(4, 4), sunColor));
            world.AddShape(new SphereShape((PlaneAxis1 + PlaneAxis2) / 2 + new Vector3D(0, 1.5f, 0), 1, (rays, dir) => { return CalculateColor(rays, 1, 1, 1, 1); }));
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(MULTI_THREADING_ENABLED, USE_ADVANCE_TRACE);
        }
    }
}