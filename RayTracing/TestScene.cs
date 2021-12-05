namespace RayTracing
{
    public class TestScene
    {
        private const bool MULTI_THREADING_ENABLED = true;
        private const bool USE_ADVANCE_TRACE = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 8;
        private const int CHUNKS_Y = 8;

        private const int RES_X = 1024;
        private const int RES_Y = 1024;

        private const int RAYS_PER_PIXEL_X = 4;
        private const int RAYS_PER_PIXEL_Y = 4;

        private const int BOUCES = 8;

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

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1f, .2f),
                new Vector3D(-10, -10, 5),
                new Vector3D(0, 5, 25),
                25,
                25,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES,
                new RTColor(),
                new RTColor());

            world.SetMainCamera(camera);

////            GlobalLight globalLight = new GlobalLight(new Vector3D(0f, 100f, 0f), new Vector3D(-0.0001f, 0, 0), new Vector3D(0, -0.0004f, -0.0001f), new Int2D(4, 4), new RTColor(255, 255, 255, 255));
            GlobalLight globalLight1 = new GlobalLight(new Vector3D(0f, 100f, 0f), new Vector3D(0f, 0, 0.0001f), new Vector3D(-0.0001f, -0.0004f, 0f), new Int2D(4, 4), new RTColor(RTColor.MAX_INTENSITY, 255, 255, 255));
            PointLight pointLight = new PointLight(new RTColor(RTColor.MAX_INTENSITY, 255, 123, 255), new Vector3D(8.5f, 1.5f, 1f));

            world.AddLightSource(globalLight1);
            world.AddLightSource(pointLight);

            world.AddShape(new SphereShape(new Vector3D(7f, 1.75f, 1f), 1f, (clrs, dir) =>
            {
                return CalculateColor(clrs, 1f, 1.5f, 1.5f, 1.5f);
            }));

            world.AddShape(new PlaneShape(new Vector3D(-10, 0.6f, -10), new Vector3D(0, 0, 1) * 50, new Vector3D(1, 0, 0) * 50, (clrs, dir) =>
            {
                return CalculateColor(clrs, 1f, 1f, 1f, 1f);
            }));
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(MULTI_THREADING_ENABLED, USE_ADVANCE_TRACE);
        }
    }
}