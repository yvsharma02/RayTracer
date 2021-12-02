namespace RayTracing
{
    public class TestScene
    {
        private const bool MULTI_THREADING_ENABLED = true;

        private const string SAVE_LOCATION = @"D:\Projects\VisualStudio\RayTracing\Generated";

        private const int CHUNKS_X = 16;
        private const int CHUNKS_Y = 16;

        private const int RES_X = 1024;
        private const int RES_Y = 1024;

        private const int RAYS_PER_PIXEL_X = 16;
        private const int RAYS_PER_PIXEL_Y = 16;

        private const int BOUCES = 6;

        private World world;

        public TestScene()
        {
            world = new World(null, null);

            Camera camera = new Camera(
                new Vector3D(1, 0, 0),
                new Vector3D(0, 1, 0),
                new Vector3D(-5, -5, 0),
                new Vector3D(0, 0, -10),
                10,
                10,
                0,
                new Int2D(RES_X, RES_Y),
                new Int2D(RAYS_PER_PIXEL_X, RAYS_PER_PIXEL_Y),
                BOUCES);

            world.SetMainCamera(camera);

            GlobalLight globalLight = new GlobalLight(new Vector3D(0f, 100f, 0f), new Vector3D(0f, -1f, 1f), new RTColor(255, 255, 0, 0));

            world.SetGlobalLightSource(globalLight);

            world.AddShape(new SphereShape(new Vector3D(0, 0, 0), 1f, (incidentClr, incidentRay, pointOfContact) =>
            {
                return new RTColor(incidentClr.Intensity / 2, incidentClr.R / 2, incidentClr.G, incidentClr.B);
            }));
        }

        public void Render()
        {
            string fileName = DateTime.Now.ToString().Replace(":", "-") + ".png";

            Renderer render = new Renderer(new Int2D(CHUNKS_X, CHUNKS_Y), world, Path.Combine(SAVE_LOCATION, fileName));
            render.Render(MULTI_THREADING_ENABLED);
        }
    }
}